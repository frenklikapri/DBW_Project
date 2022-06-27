using FileSharing.Common.Dtos.PaginatedTable;
using FileSharing.Common.Store.PaginatedTable;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharing.App.Components
{
    partial class PaginatedTable<TItem> : ComponentBase
    {
        [Inject] public IDispatcher Dispatcher { get; set; }

        [Parameter] public RenderFragment HeaderTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> RowTemplate { get; set; }
        [Parameter] public RenderFragment HeaderFiltersTemplate { get; set; }
        [Parameter] public Func<string, IQueryable<TItem>> GetItemsQuery { get; set; }
        [Parameter] public Func<PaginationParameters, Task<PaginatedListResult<TItem>>> GetItemsAsListAsync { get; set; }
        [Parameter] public bool HasPagination { get; set; } = true;
        [Parameter] public bool ShowPageSize { get; set; } = true;
        [Parameter] public bool ShowSearch { get; set; } = true;
        [Parameter] public int PageSize { get; set; } = 10;
        [Parameter] public int SearchDelayMs { get; set; } = 300;
        [Parameter] public int TotalColumns { get; set; } = 100;
        [Parameter] public string TableCssClass { get; set; } = "table";
        [Parameter] public bool UseIQueryable { get; set; } = true;
        [Parameter] public string SearchPlaceholder { get; set; } = "Search";
        [Parameter] public string SearchIconName { get; set; } = "search";

        [Parameter] public bool ShowPageNumbers { get; set; } = true;

        /// <summary>
        /// This is used to group table rows by a property. Ex. Name
        /// </summary>
        [Parameter] public string GroupBy { get; set; }

        /// <summary>
        /// Used to tell how many links will be shown Radius * 2
        /// </summary>
        [Parameter] public int Radius { get; set; } = 5;

        /// <summary>
        /// This is used to add select functionality
        /// </summary>
        [Parameter] public bool ShowCheckboxColumn { get; set; } = false;

        [Parameter] public string CheckboxColumnCssStyle { get; set; } = "";

        [Parameter] public string CheckboxColumnCssClass { get; set; } = "";

        public int CurrentPage { get; set; } = 1;
        public List<LinkModel> Links { get; set; } = new List<LinkModel>();
        public List<PaginatedTableRowDto<TItem>> List
        {
            get
            {
                return _list;
            }
        }

        public List<TItem> SelectedItems
        {
            get
            {
                try
                {
                    return _list
                        .Where(i => i.Selected)
                        .Select(i => i.Item)
                        .ToList();
                }
                catch
                {
                    return new();
                }
            }
        }

        public bool AllSelected
        {
            get
            {
                return _list?.Count > 0 && _list?.Count(i => i.Selected) == _list?.Count;
            }
            set
            {
                _list?.ForEach(i => i.Selected = value);
                InvokeAsync(StateHasChanged);
                Dispatcher.Dispatch(new PaginatedTableAction
                {

                });
            }
        }

        private int _totalAmountPages { get; set; }
        private int _totalItems { get; set; }
        private List<PaginatedTableRowDto<TItem>> _list { get; set; }
        private string _searchText = "";
        private System.Timers.Timer _searchTimer;
        private bool _loading = false;

        /// <summary>
        /// This is used only when groupby is not null or empty
        /// </summary>
        private List<PaginatedTableGroupDto<TItem>> _itemsToShow = new();

        protected override async Task OnInitializedAsync()
        {
            ResetTimer();
            await LoadItems();
        }

        public int CountPageItems()
        {
            return _list.Count();
        }

        private void CheckboxChanged(ChangeEventArgs args, PaginatedTableRowDto<TItem> item)
        {
            bool.TryParse(args.Value.ToString(), out bool value);
            item.Selected = value;
            Dispatcher.Dispatch(new PaginatedTableAction
            {

            });
        }

        public async Task LoadItemsFromParentAsync(bool goToFirstPage = true)
        {
            if (goToFirstPage)
                CurrentPage = 1;

            await LoadItems();
        }

        private async Task SelectedPageInternal(LinkModel link)
        {
            if (link.Page == CurrentPage)
            {
                return;
            }

            if (!link.Enabled)
            {
                return;
            }

            CurrentPage = link.Page;
            await LoadItems();
        }

        private void LoadPages()
        {
            Links = new List<LinkModel>();
            var isPreviousPageLinkEnabled = CurrentPage != 1;
            var previousPage = CurrentPage - 1;
            Links.Add(new LinkModel(previousPage, isPreviousPageLinkEnabled, "Previous"));

            var shouldAdd = (Radius * 2) + 1;
            var added = 0;

            var linksToAdd = new List<LinkModel>();

            for (int i = 1; i <= _totalAmountPages; i++)
            {
                if (i >= CurrentPage - Radius && i <= CurrentPage + Radius)
                {
                    linksToAdd.Add(new LinkModel(i) { Active = CurrentPage == i });
                    added++;
                }
            }

            #region If the number of links is not Radius * 2 then we add some other links to make the number of links Radius * 2

            // if total pages added are not Radius * 2 then add the remaining
            // the current page will not be in center
            if (added < shouldAdd && _totalAmountPages >= shouldAdd)
            {
                if (linksToAdd.Last().Page - CurrentPage < 5)
                {
                    for (int i = _totalAmountPages; i >= 1; i--)
                    {
                        if (!linksToAdd.Any(l => l.Page == i) && added < shouldAdd)
                        {
                            linksToAdd.Add(new LinkModel(i) { Active = CurrentPage == i });
                            added++;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= _totalAmountPages; i++)
                    {
                        if (!linksToAdd.Any(l => l.Page == i) && added < shouldAdd)
                        {
                            linksToAdd.Add(new LinkModel(i) { Active = CurrentPage == i });
                            added++;
                        }
                    }
                }
            }

            #endregion

            linksToAdd = linksToAdd
                .OrderBy(l => l.Page)
                .ToList();

            // add links of pages
            Links.AddRange(linksToAdd);

            // adding the Next button
            var isNextPageLinkEnabled = CurrentPage != _totalAmountPages && _totalItems > 0;
            var nextPage = CurrentPage + 1;
            Links.Add(new LinkModel(nextPage, isNextPageLinkEnabled, "Next"));

            // if links doesn't contain page 1 then add the first page button
            if (!Links.Any(l => l.Page == 1) && linksToAdd.Any())
            {
                Links.Insert(1, new LinkModel(1, CurrentPage != 1, "1"));
                Links.Insert(2, new LinkModel(1, false, "..."));
            }

            // if links doesn't contain last page then add the last page button
            if (!Links.Any(l => l.Page == _totalAmountPages) && linksToAdd.Any())
            {
                Links.Insert(Links.Count - 1, new LinkModel(1, false, "..."));
                Links.Insert(Links.Count - 1, new LinkModel(_totalAmountPages, CurrentPage != _totalAmountPages, _totalAmountPages.ToString()));
            }
        }

        private async Task PageSizeChange(ChangeEventArgs ev)
        {
            CurrentPage = 1;
            PageSize = int.Parse(ev.Value.ToString());
            await LoadItems();
        }

        async Task LoadItems()
        {
            try
            {
                _loading = true;
                InvokeAsync(StateHasChanged);
                if (UseIQueryable)
                {
                    _totalItems = await GetItemsQuery.Invoke(_searchText).CountAsync();

                    // get all if PageSize == -1
                    if (PageSize == -1)
                    {
                        _totalAmountPages = 1;

                        var list = await GetItemsQuery
                            .Invoke(_searchText)
                            .ToListAsync();

                        _list = list.Select(i => new PaginatedTableRowDto<TItem>
                        {
                            Item = i,
                            Selected = false,
                            Show = true
                        }).ToList();
                    }
                    else
                    {
                        _totalAmountPages = (int)Math.Ceiling((double)_totalItems / PageSize);

                        var list = await GetItemsQuery
                            .Invoke(_searchText)
                            .Skip((CurrentPage - 1) * PageSize)
                            .Take(PageSize)
                            .ToListAsync();

                        _list = list.Select(i => new PaginatedTableRowDto<TItem>
                        {
                            Item = i,
                            Selected = false,
                            Show = true
                        }).ToList();
                    }
                }
                else
                {
                    var paginationParams = new PaginationParameters
                    {
                        Page = CurrentPage,
                        PageSize = PageSize,
                        Search = _searchText
                    };
                    var result = await GetItemsAsListAsync(paginationParams);
                    var list = result.Items;
                    _list = list.Select(i => new PaginatedTableRowDto<TItem>
                    {
                        Item = i,
                        Selected = false,
                        Show = true
                    }).ToList();

                    _totalItems = result.CountAll;
                    if (PageSize == -1)
                    {
                        _totalAmountPages = 1;
                    }
                    else
                    {
                        _totalAmountPages = (int)Math.Ceiling((double)_totalItems / PageSize);
                    }
                }

                if (!string.IsNullOrEmpty(GroupBy))
                {
                    var groups = List
                        .GroupBy(c => c.Item.GetType().GetProperty(GroupBy).GetValue(c.Item, null))
                        .Select(c => new PaginatedTableGroupDto<TItem>
                        {
                            Items = c.ToList(),
                            Name = c.Key.ToString(),
                            Show = true
                        })
                        .ToList();

                    _itemsToShow = groups;
                }

                LoadPages();
                _loading = false;
                InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                _loading = false;
                InvokeAsync(StateHasChanged);
            }
        }

        async Task SearchOnChange(ChangeEventArgs ev)
        {
            CurrentPage = 1;
            var value = ev.Value.ToString().ToLower();

            _searchText = value;

            _searchTimer.Stop();

            _searchTimer.Start();
        }

        void ResetTimer()
        {
            _searchTimer = new System.Timers.Timer(SearchDelayMs);
            _searchTimer.Elapsed += OnUserFinish;
            _searchTimer.AutoReset = false;
        }

        private async void OnUserFinish(Object source, System.Timers.ElapsedEventArgs e)
        {
            _searchTimer?.Dispose();
            ResetTimer();
            await LoadItems();
        }
    }

    public class LinkModel
    {
        public LinkModel(int page)
            : this(page, true) { }

        public LinkModel(int page, bool enabled)
            : this(page, enabled, page.ToString())
        { }

        public LinkModel(int page, bool enabled, string text)
        {
            Page = page;
            Enabled = enabled;
            Text = text;
        }

        public string Text { get; set; }
        public int Page { get; set; }
        public bool Enabled { get; set; } = true;
        public bool Active { get; set; } = false;
    }
}

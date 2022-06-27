using Fluxor;

namespace FileSharing.App.Store.Identity
{
    public class IdentityFeature : Feature<IdentityState>
    {
        public override string GetName() => "Identity";

        protected override IdentityState GetInitialState() => new();
    }
}

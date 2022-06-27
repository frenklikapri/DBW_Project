using Fluxor;

namespace FileSharing.App.Store.Identity
{
    public class IdentityReducer : Reducer<IdentityState, IdentityAction>
    {
        public override IdentityState Reduce(IdentityState state, IdentityAction action)
        {
            //maybe change the state
            return new IdentityState();
        }
    }
}

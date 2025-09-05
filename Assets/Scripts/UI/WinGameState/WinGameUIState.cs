
namespace SGGames.Scripts.UI
{
    public enum WinGameUIStateType
    {
        PickNewItem,
        PickNewCard,
    }
    
    public abstract class WinGameUIState
    {
        protected WinGameUIStateType m_stateType;
        
        public WinGameUIStateType StateType => m_stateType;

        public WinGameUIState(WinGameUIStateType stateType)
        {
            m_stateType = stateType;
        }
        public abstract void Initialize();
        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void Hide();
    }
}

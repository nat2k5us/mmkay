namespace Client.Entities
{
    using Core.Common.Core;

    public class Sign : ObjectBase
    {
        private int signId;
        public int SignId
        {
            get { return signId; }
            set
            {
                if (signId != value)
                {
                    signId = value;
                    OnPropertyChanged(() => SignId);
                }
            }
        }
    }
}

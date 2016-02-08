using EnsureThat;

namespace BuildHelper.Build
{
    internal class TrackInfo
    {
        private readonly string m_Id;
        private readonly bool m_Restart;

        public TrackInfo(string id, bool restart)
        {
            Ensure.That(() => id).IsNotNullOrEmpty();
            m_Restart = restart;
            m_Id = id;
        }

        public string Id
        {
            get
            {
                return m_Id;
            }
        }

        public bool Restart
        {
            get
            {
                return m_Restart;
            }
        }
    }
}

namespace Betauer.Loader {
    public class LoadingProgress {
        public int TotalSize { get; private set; }
        public int TotalLoadedSize { get; private set; }
        public float TotalLoadedPercent => (float)TotalLoadedSize / TotalSize;

        public string ResourcePath { get; private set; }
        public int ResourceSize { get; private set; }
        public int ResourceLoadedSize { get; private set; }
        public float ResourceLoadedPercent => (float)ResourceLoadedSize / ResourceSize;

        public LoadingProgress Update(int totalSize, int totalLoadedSize, string resourcePath, int resourceSize,
            int resourceLoadedSize) {
            TotalSize = totalSize;
            TotalLoadedSize = totalLoadedSize;
            ResourcePath = resourcePath;
            ResourceSize = resourceSize;
            ResourceLoadedSize = resourceLoadedSize;
            return this;
        }
    }
}
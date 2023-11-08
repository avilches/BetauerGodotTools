namespace Betauer.Core.Data;

public interface INormalizedDataGrid<out T> : IDataGrid<T> {
    public void Load();
}
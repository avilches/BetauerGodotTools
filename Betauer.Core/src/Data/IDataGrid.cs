namespace Betauer.Core.Data;

public interface IDataGrid<out T> {
    public T GetValue(int x, int y);
    public void Resize(int width, int height);
    public void Load();
}
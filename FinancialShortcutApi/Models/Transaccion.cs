namespace FinancialShortcutApi.Models;

public class Transaccion
{
    public int Id { get; set; }
    public string TipoMovimiento { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public DateTime Fecha { get; set; }
}

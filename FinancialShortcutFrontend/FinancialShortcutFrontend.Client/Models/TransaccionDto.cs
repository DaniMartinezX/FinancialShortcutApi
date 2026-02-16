namespace FinancialShortcutFrontend.Client.Models;

public class TransaccionDto
{
    public string tipoMovimiento { get; set; } = string.Empty;
    public string categoria { get; set; } = string.Empty;
    public decimal cantidad { get; set; }
    public DateTime fecha { get; set; }
}

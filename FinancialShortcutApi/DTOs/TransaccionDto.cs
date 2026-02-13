namespace FinancialShortcutApi.DTOs;

public class TransaccionDto
{
    public string tipo_movimiento { get; set; } = string.Empty;
    public string categoria { get; set; } = string.Empty;
    public decimal cantidad { get; set; }
}

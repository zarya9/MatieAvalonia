namespace MatieAvalonia.Models;

/// <summary>Элемент списка «карта для пополнения»: <see cref="IdCard"/> = 0 — привязка новой карты.</summary>
public sealed class CardPickListItem
{
    public int IdCard { get; init; }

    public string Display { get; init; } = "";
}

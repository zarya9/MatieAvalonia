namespace MatieAvalonia.Classes;

/// <summary>Ограничения полей как в <see cref="Data.AppDbContext"/> (PostgreSQL / EF).</summary>
public static class DbFieldLimits
{
    public const int UserFio = 50;
    public const int UserLogin = 50;
    public const int UserPassword = 50;

    public const int ServiceName = 50;
    public const int ServiceDescription = 50;
    /// <summary><c>numeric(7,2)</c> — максимум 99999.99.</summary>
    public const decimal ServicePriceMax = 99999.99m;

    public const int ReviewComment = 255;

    public const int CardNumber = 20;
    public const int CardCvv = 3;

    /// <summary><c>numeric(20,2)</c> у пользователя — 18 знаков до запятой, 2 после.</summary>
    public const decimal BalanceMonetaryMax = 999999999999999999.99m;

    /// <summary>Поиск по каталогу (не хранится в БД как отдельное поле).</summary>
    public const int CatalogSearch = 100;
}

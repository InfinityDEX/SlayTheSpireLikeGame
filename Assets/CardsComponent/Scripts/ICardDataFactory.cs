using System.Collections.Generic;

/// <summary>
/// カードデータファクトリーインターフェース
/// </summary>
/// <returns></returns>
public interface ICardDataFactory
{
    /// <summary>
    /// カードデータを生成する
    /// </summary>
    /// <returns>カードデータ</returns>
    List<CardData> CreateCardDataList();
}

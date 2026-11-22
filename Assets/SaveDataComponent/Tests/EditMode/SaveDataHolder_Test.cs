using NUnit.Framework;
using System;
using UnityEngine;

namespace SaveDataComponent.Tests.EditMode
{
    /// <summary>
    /// セーブデータホルダーのテスト定義
    /// </summary>
    public class SaveDataHolder_Test
    {
        /// <summary>
        /// テスト対象クラス
        /// </summary>
        private SaveDataHolder saveDataHolderInstance;

        /// <summary>
        /// SaveDataHolderを登録するゲームオブジェクト
        /// </summary>
        private GameObject go;

        /// <summary>
        /// セーブデータ(既定値)
        /// 
        /// SaveDataEntityの既定値が変わってしまっても問題ないようにテスト側が前提としている値はここで定義する
        /// </summary>
        private readonly SaveDataEntity defaultSaveData = new SaveDataEntity()
        {
            maxHealth = 100,
            currentHealth = 100,
            currentFloor = 1,
            money = 0 // 所持金はテストしやすいように0に設定
        };

        /// <summary>
        /// 本テストスコープ開始時に一度だけ呼び出すセットアップ
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            go = new();
        }

        /// <summary>
        /// セットアップ（毎テスト開始前実行）
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // SaveDataHolderの生成
            saveDataHolderInstance = go.AddComponent<SaveDataHolder>();

            // 依存性注入（DI）
            // 記載をシンプルにする為、VContainerのビルダーやコンテナは使用せず、
            // DI用のメソッドを呼び出して渡す
            saveDataHolderInstance.Init(new SimpleSaveDataIO(defaultSaveData)); 
        }

        /// <summary>
        /// テスト用ファイル取り壊し（テストスコープの最後に一度だけ）
        /// </summary>
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // 使いまわしていたgoを削除
            UnityEngine.Object.DestroyImmediate(go);
        }

        /// <summary>
        /// テスト用ファイル取り壊し（毎テスト開始後実行）
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // テストに使用したsaveDataHolderは毎回削除する
            UnityEngine.Object.DestroyImmediate(saveDataHolderInstance);
        }

        /// <summary>
        /// 所持金を追加できることを確認
        /// </summary>
        /// <param name="amount">消費したい金額</param>
        [Test]
        public void AddMoney(
            [Values(0, 1, 20, 36, 100)] int amount)
        {
            saveDataHolderInstance.LoadSaveData();

            saveDataHolderInstance.AddMoney(amount);
            // amountの値が現在の所持金であることを確認（テスト中所持金の初期値を0に設定している為）
            Assert.That(saveDataHolderInstance.SaveData.money, Is.EqualTo(amount));
        }

        /// <summary>
        /// 所持金追加メソッドに負の値をセットしたら例外が出ることを確認
        /// </summary>
        /// <param name="amount">消費したい金額</param>
        [Test]
        public void AddNegativeNumbersMoney(
            [Values(-1, -20, -36, -100)] int amount)
        {
            saveDataHolderInstance.LoadSaveData();

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => saveDataHolderInstance.AddMoney(amount));

            Assert.That(ex.Message, Does.StartWith($"追加したい金額として[{amount}]が指定されました。負の数は使用できません。"));
        }

        /// <summary>
        /// 所持金以内の金額であれば所持金を消費できることを確認
        /// </summary>
        /// <param name="cost">消費したい金額</param>
        [Test]
        public void UseableMoney(
            [Values(0, 1, 20, 36, 100)] int cost)
        {
            saveDataHolderInstance.LoadSaveData();

            saveDataHolderInstance.AddMoney(100);
            // 追加した100マネーが現在の所持金であることを確認（テスト中所持金の初期値を0に設定している為）
            Assert.That(saveDataHolderInstance.SaveData.money, Is.EqualTo(100));

            saveDataHolderInstance.UseMoney(cost); // 0以上100以下であれば例外にならず消費できる

            // 追加した100マネーからcostとして指定した値分引かれた値が現在の所持金であることを確認
            Assert.That(saveDataHolderInstance.SaveData.money, Is.EqualTo(100 - cost));
        }

        /// <summary>
        /// 所持金以内の金額であれば、支払可能確認処理がtrueを返すことを確認
        /// </summary>
        /// <param name="cost">消費できるか確認したい金額</param>
        [Test]
        public void CanAffordMoney(
            [Values(0, 1, 20, 36, 100)] int cost)
        {
            saveDataHolderInstance.LoadSaveData();

            saveDataHolderInstance.AddMoney(100);
            // 追加した100マネーが現在の所持金であることを確認（テスト中所持金の初期値を0に設定している為）
            Assert.That(saveDataHolderInstance.SaveData.money, Is.EqualTo(100));

            // costが100以下の整数ならtrueを返すことを確認する
            Assert.That(saveDataHolderInstance.CanAfford(cost), Is.True);
        }

        /// <summary>
        /// 所持金以上の金額を指定して支払処理をしようとしたら例外が返ることを確認
        /// </summary>
        /// <param name="amount">支払い前の所持金</param>
        /// <param name="cost">消費したい金額</param>
        [Test]
        public void UseExcessionMoney(
            [Values(20, 99, 100)] int amount,
            [Values(101, 120, 1000)] int cost)
        {
            saveDataHolderInstance.LoadSaveData();

            saveDataHolderInstance.AddMoney(amount);
            // 追加したamountの値が現在の所持金であることを確認（テスト中所持金の初期値を0に設定している為）
            Assert.That(saveDataHolderInstance.SaveData.money, Is.EqualTo(amount));

            var ex = Assert.Throws<InvalidOperationException>(() => saveDataHolderInstance.UseMoney(cost));

            Assert.That(ex.Message, Is.EqualTo($"Moneyを[{cost}]消費しようとしました。現在の所持金は[{amount}]でした。"));
        }

        /// <summary>
        /// 所持金以上の金額を指定して支払処理をしようとしたらfalse（支払不能）が返ることを確認
        /// </summary>
        /// <param name="amount">支払い前の所持金</param>
        /// <param name="cost">消費できるか確認したい金額</param>
        [Test]
        public void CantAffordMoney(
            [Values(20, 99, 100)] int amount,
            [Values(101, 120, 1000)] int cost)
        {
            saveDataHolderInstance.LoadSaveData();

            saveDataHolderInstance.AddMoney(amount);
            // 追加したamountの値が現在の所持金であることを確認（テスト中所持金の初期値を0に設定している為）
            Assert.That(saveDataHolderInstance.SaveData.money, Is.EqualTo(amount));

            // 支払不可（false）と返されることを確認
            Assert.That(saveDataHolderInstance.CanAfford(cost), Is.False);
        }

        /// <summary>
        /// 負の数の金額を指定して支払処理をしようとしたら例外が返ることを確認
        /// </summary>
        /// <param name="cost">消費したい金額</param>
        [Test]
        public void UseNegativeNumbersMoney(
            [Values(-1, -20, -36, -100)] int cost)
        {
            saveDataHolderInstance.LoadSaveData();

            saveDataHolderInstance.AddMoney(100);
            // 追加した100マネーが現在の所持金であることを確認（テスト中所持金の初期値を0に設定している為）
            Assert.That(saveDataHolderInstance.SaveData.money, Is.EqualTo(100));

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => saveDataHolderInstance.UseMoney(cost));

            Assert.That(ex.Message, Does.StartWith($"消費したい金額として[{cost}]が指定されました。負の数は使用できません。"));
        }


        /// <summary>
        /// 負の数の金額を指定して支払可能判定処理をしようとしたら例外が返ることを確認
        /// </summary>
        /// <param name="cost">消費できるか確認したい金額</param>
        [Test]
        public void CanAffordNegativeNumbersMoney(
            [Values(-1, -20, -36, -100)] int cost)
        {
            saveDataHolderInstance.LoadSaveData();

            saveDataHolderInstance.AddMoney(100);
            // 追加した100マネーが現在の所持金であることを確認（テスト中所持金の初期値を0に設定している為）
            Assert.That(saveDataHolderInstance.SaveData.money, Is.EqualTo(100));

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => saveDataHolderInstance.CanAfford(cost));

            Assert.That(ex.Message, Does.StartWith($"消費できるか確認したい金額として[{cost}]が指定されました。負の数は使用できません。"));
        }
    }
}
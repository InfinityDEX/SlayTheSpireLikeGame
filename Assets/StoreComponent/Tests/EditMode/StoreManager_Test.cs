using NUnit.Framework;
using UnityEngine;

namespace StoreComponent.Tests.EditMode
{
    public class StoreManager_Test
    {
        /// <summary>
        /// テスト対象クラス
        /// </summary>
        private StoreManager storeManagerInstance;

        /// <summary>
        /// StoreManagerを登録するゲームオブジェクト
        /// </summary>
        private GameObject go;

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
            // StoreManagerの生成
            storeManagerInstance = go.AddComponent<StoreManager>();

            // 依存性注入（DI）
            // 記載をシンプルにする為、VContainerのビルダーやコンテナは使用せず、
            // DI用のメソッドを呼び出して渡す
            //storeManagerInstance.Init();
        }

        /// <summary>
        /// テスト用ファイル取り壊し（テストスコープの最後に一度だけ）
        /// </summary>
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // 使いまわしていたgoを削除
            Object.DestroyImmediate(go);
        }

        /// <summary>
        /// テスト用ファイル取り壊し（毎テスト開始後実行）
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // テストに使用したstoreManagerInstanceは毎回削除する
            Object.DestroyImmediate(storeManagerInstance);
        }
    }
}
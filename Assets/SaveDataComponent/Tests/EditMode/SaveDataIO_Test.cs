using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.TestTools;

namespace SaveDataComponent.Tests.EditMode
{
    public class SaveDataIO_Test
    {
        /// <summary>
        /// テスト用ファイル格納先パス（テスト終了後自動削除）
        /// </summary>
        private readonly string rootFolderPath = Path.Combine(Path.GetTempPath(), "SaveDataIO_Test");

        /// <summary>
        /// ファイル名
        /// </summary>
        private const string fileName = "tmpSaveData.json";

        /// <summary>
        /// フォルダパス
        /// </summary>
        private string folderPath;

        /// <summary>
        /// テスト用ファイル取り壊し
        /// </summary>
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // テスト用ファイルの入ったディレクトリごと全て削除する。
            if (Directory.Exists(rootFolderPath))
            {
                Directory.Delete(rootFolderPath, true);
            }
        }

        /// <summary>
        /// セットアップ
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // フォルダパスを指定(テストの度に別のフォルダを用意するのでテスト前に毎回設定する)
            folderPath = Path.Combine(rootFolderPath, Guid.NewGuid().ToString());
        }

        /// <summary>
        /// 保存して読み込むと、同じ内容に戻ることを確認
        /// </summary>
        [Test]
        public void SaveAndLoad()
        {
            SaveDataIO saveDataIOInstance = new(folderPath, fileName);

            // 規定値と中身のデータが異なるSaveDataEntityを作成
            SaveDataEntity saveDataEntity = new()
            {
                maxHealth = 19,
                currentHealth = 10,
                money = 500,
                currentFloor = 3
            };
            // 一旦保存
            saveDataIOInstance.SaveData(saveDataEntity);

            // 一度SaveDataIO内にある情報を使いまわしていないことを保証する為にインスタンスを生成し直す
            saveDataIOInstance = new(folderPath, fileName);

            // セーブデータファイルを読み込む
            var loadData = saveDataIOInstance.LoadData();

            // 内容が変わっていないことを確認する。
            Assert.That(loadData.maxHealth, Is.EqualTo(saveDataEntity.maxHealth));
            Assert.That(loadData.currentHealth, Is.EqualTo(saveDataEntity.currentHealth));
            Assert.That(loadData.currentFloor, Is.EqualTo(saveDataEntity.currentFloor));
            Assert.That(loadData.money, Is.EqualTo(saveDataEntity.money));
        }

        /// <summary>
        /// ファイル読み込みメソッドを実行してロードができなかったら（指定パスに指定ファイルがなかったら）
        /// SaveDataEntityの既定値が返ることを確認
        /// </summary>
        [Test]
        public void CreateDefaultSaveDataIfTargetFileNotFound()
        {
            SaveDataIO saveDataIOInstance = new(folderPath, fileName);
            // 既定値でSaveDataEntityインスタンスを作成
            SaveDataEntity defaultSaveDataEntity = new();

            // セーブデータファイルを読み込む
            // 
            // このテスト用フォルダにセーブデータを保存していないので
            // ロードしたら既定値のSaveDataEntityが返るはず。
            var loadData = saveDataIOInstance.LoadData();

            // フォルダが作成されていないことを確認する
            // ※セーブデータを保存するまで不要なフォルダである為、
            // ロード時にファイルがない分にはフォルダ作成をしない
            Assert.That(Directory.Exists(folderPath), Is.False);

            // 生成されたSaveDataEntityの内容が既定値であることを確認する
            Assert.That(loadData.maxHealth, Is.EqualTo(defaultSaveDataEntity.maxHealth));
            Assert.That(loadData.currentHealth, Is.EqualTo(defaultSaveDataEntity.currentHealth));
            Assert.That(loadData.currentFloor, Is.EqualTo(defaultSaveDataEntity.currentFloor));
            Assert.That(loadData.money, Is.EqualTo(defaultSaveDataEntity.money));

        }

        /// <summary>
        /// ロードしたファイルの中身が空っぽだった場合既定値を返すことを確認
        /// </summary>
        [Test]
        public void CreateDefaultSaveDataIfEmptyFileLoad()
        {
            // Jsonに何も書き込まず空テキストで保存する
            Directory.CreateDirectory(folderPath);
            File.WriteAllText(Path.Combine(folderPath, fileName), "");
            SaveDataIO saveDataIOInstance = new(folderPath, fileName);
            // 既定値でSaveDataEntityインスタンスを作成
            SaveDataEntity defaultSaveDataEntity = new();

            // 中身が空っぽのファイルを読み込んだことによりセーブデータを新規作成した場合、
            // 空っぽだったファイルのパスが記載された警告ログが出ていることを確認する
            // （LogAssert.Expectは実際にログが呼び出される前に定義するのでここに記載）
            LogAssert.Expect(
                UnityEngine.LogType.Warning,
                new Regex(Regex.Escape(Path.Combine(folderPath, fileName))));

            // セーブデータファイルを読み込む
            // 
            // 中身が空のファイルを読み込んだら、
            // ファイルがない時と同じ扱いにするので既定値が返るはず
            var loadData = saveDataIOInstance.LoadData();

            // 生成されたSaveDataEntityの内容が既定値であることを確認する
            Assert.That(loadData.maxHealth, Is.EqualTo(defaultSaveDataEntity.maxHealth));
            Assert.That(loadData.currentHealth, Is.EqualTo(defaultSaveDataEntity.currentHealth));
            Assert.That(loadData.currentFloor, Is.EqualTo(defaultSaveDataEntity.currentFloor));
            Assert.That(loadData.money, Is.EqualTo(defaultSaveDataEntity.money));
        }

        /// <summary>
        /// ファイル書き込み時に指定パスのフォルダが無かったら
        /// 指定パスのフォルダを作成してからファイルをそこに保存することを確認
        /// </summary>
        [Test]
        public void CreateFolderOnSaveIfFolderMissing()
        {
            SaveDataIO saveDataIOInstance = new(folderPath, fileName);
            SaveDataEntity saveDataEntity = new();

            // 保存する
            saveDataIOInstance.SaveData(saveDataEntity);

            // フォルダが作成されていることを確認する
            Assert.That(Directory.Exists(folderPath), Is.True);
            // 一応ファイルが作成されていることも確認する
            Assert.That(File.Exists(Path.Combine(folderPath, fileName)), Is.True);
        }

        /// <summary>
        /// ロードしたファイルの中身をSaveDataEntityにコンバート出来なかった場合例外になることを確認
        /// </summary>
        [Test]
        public void ExceptionIfCorruptedFileLoad()
        {
            // Jsonに変換不可能なテキストを書き込んで保存する
            Directory.CreateDirectory(folderPath);
            File.WriteAllText(Path.Combine(folderPath, fileName), "asdjasf;kljasdkljaswd cas sadf kl");
            SaveDataIO saveDataIOInstance = new(folderPath, fileName);

            // 読み込めないファイルなのでエラーが発生するはず
            var ex = Assert.Throws<InvalidDataException>(() => saveDataIOInstance.LoadData());

            // 例外メッセージにファイルパスが記載されていることを確認する
            Assert.That(ex.Message, Does.Contain(Path.Combine(folderPath, fileName)));
        }
    }
}
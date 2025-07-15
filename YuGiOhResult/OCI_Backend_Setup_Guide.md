# OCIバックエンドAPI構築ガイド

OCIのAlways Free枠を活用し、**OCI Functions**, **API Gateway**, **Vault** を使ってバックエンドAPIを構築する手順です。

---

### 概要：これから作ること

1.  **OCIの準備（インフラ構築）:**
    *   **Vault**に秘密鍵を安全に保管します。
    *   **IAMポリシー**で、これから作るFunction（APIのプログラム）がVaultとObject Storageにアクセスする権限を設定します。
2.  **バックエンドAPIの開発（C# Function）:**
    *   ローカルPCで、OCI Functionsのプロジェクトを作成します。
    *   MAUIアプリからデータを受け取り、Vaultから秘密鍵を読み出し、Object StorageにファイルをアップロードするC#コードを書きます。
    *   完成したコードをOCIにデプロイします。
3.  **API Gatewayの設定（APIの公開）:**
    *   デプロイしたFunctionを、インターネットから呼び出せる安全なURL（エンドポイント）として公開します。
4.  **MAUIアプリの修正（APIの呼び出し）:**
    *   既存のOCI SDKのロジックを削除し、新しく作ったAPIのURLを呼び出すシンプルなコードに書き換えます。

---

### フェーズ1：OCIコンソールでの準備

#### ステップ1：OCI Vaultに秘密鍵を登録する

1.  OCIコンソールメニューから **「アイデンティティとセキュリティ」** > **「Vault」** を選択します。
2.  **「ボールトの作成」** をクリックし、適当な名前（例: `YugiohAppVault`）を付けて作成します。必ず「常時無料の対象」オプションが選択されていることを確認してください。
3.  作成したVaultの名前をクリックして詳細画面に入ります。
4.  左側のリソースメニューから **「シークレット」** を選択し、**「シークレットの作成」** をクリックします。
5.  以下の通り入力します。
    *   **名前:** `OciPrivateKey`など分かりやすい名前
    *   **キー:** （空欄のままでOK）
    *   **シークレット・コンテンツ・タイプ:** **プレーン・テキスト** を選択
    *   **シークレット・コンテンツ:** あなたのPCの `C:\Users\User\.oci` フォルダにある **秘密鍵ファイル（.pem）の中身をすべてコピーして貼り付け** ます。（`-----BEGIN PRIVATE KEY-----` から `-----END PRIVATE KEY-----` まで全部です）
6.  **「シークレットの作成」** をクリックします。
7.  作成後、シークレットの詳細画面で **OCID** をコピーして、メモ帳などに控えておきます。

#### ステップ2：IAMポリシーでFunctionに権限を与える

1.  **「アイデンティティとセキュリティ」** > **「ドメイン」** > **Default** > **「動的グループ」** を選択します。
2.  **「動的グループの作成」** をクリックします。
    *   **名前:** `YugiohFunctionsGroup` など
    *   **ルール1:** `ALL {resource.type = 'fnfunc'}`
3.  次に、**「アイデンティティとセキュリティ」** > **「ポリシー」** を選択します。
4.  **「ポリシーの作成」** をクリックします。
    *   **名前:** `YugiohFunctionsPolicy` など
    *   **ポリシー・ビルダー:** 「拡張オプションの表示」をクリックし、以下のポリシー・ステートメントをテキストボックスに貼り付けます。

    ```
    # FunctionがVaultから秘密鍵を読み取ることを許可
    Allow dynamic-group YugiohFunctionsGroup to read secret-bundles in compartment id <コンパートメントのOCID> where target.secret.id = '<ステップ1で控えたシークレットのOCID>'

    # FunctionがObject Storageバケットにオブジェクトを書き込むことを許可
    Allow dynamic-group YugiohFunctionsGroup to manage objects in compartment id <コンパートメントのOCID> where target.bucket.name = 'YuGiOhCounter_Backet'
    ```
    *   `<コンパートメントのOCID>` は、あなたの環境のOCIDに置き換えてください。
    *   `<ステップ1で控えたシークレットのOCID>` は、先ほど控えたVaultシークレットのOCIDに置き換えます。

---

### フェーズ2：バックエンドAPI（Function）の開発

#### ステップ3：ローカル開発環境のセットアップ

*   **Visual Studio Code**
*   **Docker Desktop**
*   **OCI CLI**

#### ステップ4：Functionプロジェクトの作成

1.  コマンドプロンプトで作業フォルダに移動します。
    ```bash
    mkdir C:\Users\User\source\repos\YugiohApi
    cd C:\Users\User\source\repos\YugiohApi
    ```
2.  プロジェクトを初期化します。
    ```bash
    fn init --runtime dotnet8 yugioh-uploader-func
    ```

#### ステップ5：Functionのコードを実装する

1.  VS Codeで `yugioh-uploader-func` フォルダを開きます。
2.  **`yugioh-uploader-func.csproj`** にパッケージ参照を追加します。
    ```xml
    <ItemGroup>
      <PackageReference Include="OCI.DotNetSDK.Common" Version="88.0.0" />
      <PackageReference Include="OCI.DotNetSDK.Objectstorage" Version="88.0.0" />
      <PackageReference Include="OCI.DotNetSDK.Vault" Version="88.0.0" />
      <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    </ItemGroup>
    ```
3.  **`Func.cs`** の中身をすべて置き換えます。

    ```csharp
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Oci.Common.Auth;
    using Oci.Objectstorage.Transfer;
    using Oci.Vault.Model;
    using Oci.Vault.Requests;
    using Oci.Vault;
    using Oci.Objectstorage;

    namespace Fn
    {
        public class FunctionInput
        {
            public string FileName { get; set; }
            public string Content { get; set; }
        }

        public class Func
        {
            private ObjectStorageClient _objectStorageClient;
            private const string OCI_NAMESPACE = "▼▼▼あなたのOCIネームスペース▼▼▼";
            private const string BUCKET_NAME = "YuGiOhCounter_Backet";

            public Func()
            {
                InitializeObjectStorageClient().Wait();
            }

            private async Task InitializeObjectStorageClient()
            {
                try
                {
                    var provider = new ResourcePrincipalAuthenticationDetailsProvider();
                    var vaultClient = new VaultsClient(new Oci.Common.Region("ap-tokyo-1"), provider);
                    var secretId = Environment.GetEnvironmentVariable("OCI_PRIVATE_KEY_OCID");
                    
                    var getSecretRequest = new GetSecretBundleRequest { SecretId = secretId };
                    var secretBundle = await vaultClient.GetSecretBundle(getSecretRequest);

                    var base64Secret = (secretBundle.SecretBundleContent as Base64SecretBundleContentDetails);
                    var privateKeyBytes = Convert.FromBase64String(base64Secret.Content);
                    var privateKey = Encoding.UTF8.GetString(privateKeyBytes);

                    var user = Environment.GetEnvironmentVariable("OCI_USER_OCID");
                    var fingerprint = Environment.GetEnvironmentVariable("OCI_FINGERPRINT");
                    var tenancy = Environment.GetEnvironmentVariable("OCI_TENANCY_OCID");
                    var region = Oci.Common.Region.AP_TOKYO_1;

                    var stringProvider = new StringPrivateKeySupplier(privateKey);
                    var configProvider = new ConfigFileAuthenticationDetailsProvider(user, fingerprint, tenancy, region, stringProvider);
                    
                    _objectStorageClient = new ObjectStorageClient(configProvider);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Initialization failed: {ex.Message}");
                    throw;
                }
            }

            public async Task<string> HandleAsync(FunctionInput input)
            {
                if (_objectStorageClient == null) return "Error: ObjectStorageClient not initialized.";
                if (string.IsNullOrEmpty(input?.FileName) || string.IsNullOrEmpty(input.Content)) return "Error: Invalid input.";

                try
                {
                    var contentBytes = Encoding.UTF8.GetBytes(input.Content);
                    using (var memoryStream = new MemoryStream(contentBytes))
                    {
                        var request = new Oci.Objectstorage.Requests.PutObjectRequest
                        {
                            NamespaceName = OCI_NAMESPACE,
                            BucketName = BUCKET_NAME,
                            ObjectName = input.FileName,
                            PutObjectBody = memoryStream
                        };
                        await _objectStorageClient.PutObject(request);
                    }
                    return $"Success: {input.FileName} uploaded.";
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    return $"Error: {e.Message}";
                }
            }
        }
    }
    ```

#### ステップ6：FunctionをOCIにデプロイする

1.  OCIコンテキストを設定します。
    ```bash
    oci setup bootstrap --profile <あなたのOCIプロファイル名> --auth resource-principal
    ```
2.  Functionアプリケーションを作成します。
    ```bash
    fn create app yugioh-api-app --subnet-id <あなたのサブネットのOCID>
    ```
3.  Functionをデプロイします。
    ```bash
    fn -v deploy --app yugioh-api-app --no-bump `
      --build-arg "OCI_PRIVATE_KEY_OCID=▼▼▼ステップ1で控えたシークレットのOCID▼▼▼" `
      --build-arg "OCI_USER_OCID=▼▼▼あなたのユーザーOCID▼▼▼" `
      --build-arg "OCI_FINGERPRINT=▼▼▼あなたのAPIキーのフィンガープリント▼▼▼" `
      --build-arg "OCI_TENANCY_OCID=▼▼▼あなたのテナンシーOCID▼▼▼"
    ```

---

### フェーズ3：API GatewayでFunctionを公開する

#### ステップ7：API Gatewayを作成してデプロイメントを設定する

1.  OCIコンソールで **「開発者サービス」** > **「APIゲートウェイ」** を選択し、ゲートウェイを作成します。
2.  デプロイメントを作成します。
    *   **名前:** `v1`
    *   **パス接頭辞:** `/yugioh`
    *   **実行バックエンド:** **Oracle Functions** を選択し、作成したFunction (`yugioh-uploader-func`) を指定します。
3.  ルートを設定します。
    *   **パス:** `/upload`
    *   **メソッド:** **POST**
4.  作成後、**エンドポイントURL**を控えておきます。

---

### フェーズ4：MAUIアプリの修正

#### ステップ8：`ViewModelBase.cs` を修正する

1.  `ViewModelBase.cs` を開き、OCI関連の`using`ディレクティブを削除します。
2.  `UploadJsonToOCIAsync` メソッドを以下の内容に置き換えます。

    ```csharp
    // OCIにJSONデータをアップロードする (新しいバックエンドAPI版)
    protected async Task UploadJsonToOCIAsync(FileType fileType)
    {
        // API GatewayのエンドポイントURL
        var apiEndpoint = "▼▼▼ステップ7で控えたAPI GatewayのエンドポイントURL▼▼▼/upload";

        try
        {
            var jsonPath = fileType == FileType.Decks ? decksDataPath : matchesDataPath;
            var fileName = fileType == FileType.Decks ? "decks.json" : "matches.json";

            if (!File.Exists(jsonPath)) return;

            var jsonContent = await File.ReadAllTextAsync(jsonPath);

            var payload = new
            {
                FileName = fileName,
                Content = jsonContent
            };
            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var response = await client.PostAsync(apiEndpoint, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ API Success: {responseBody}");
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ API Error: {response.StatusCode}, Content: {errorBody}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception: {ex.Message}");
        }
    }
    ```
    *   `▼▼▼` の部分をあなたの情報に置き換えてください。

```
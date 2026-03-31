using Dapper; // ← これを一番上に追加！
using Npgsql; // ← これも必要です

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// appsettings.json から接続文字列を読み込む(2026.03.19 Add)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var app = builder.Build();

try 
{
    using var connection = new NpgsqlConnection(connectionString);
    
    // analysis_resultsテーブルに起動ログを記録（テーブルがない場合は作成）
    string sql = @"
        CREATE TABLE IF NOT EXISTS connection_log (
            id SERIAL PRIMARY KEY,
            message TEXT,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
        );
        INSERT INTO connection_log (message) VALUES ('C# App Connection Success!');";
    
    connection.Execute(sql);
    Console.WriteLine(">>> DB Connection Check: SUCCESS!");
}
catch (Exception ex) 
{
    Console.WriteLine($">>> DB Connection Check: FAILED! - {ex.Message}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


#if DEBUG
// --- ここからテストコード ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Dapperを使って、DBの現在時刻を聞いてみる
        using var connection = new Npgsql.NpgsqlConnection(connectionString);
        var dbTime = Dapper.SqlMapper.ExecuteScalar<DateTime>(connection, "SELECT NOW()");

        Console.WriteLine($"✅ DB接続成功！ 現在のDB時刻: {dbTime}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ DB接続エラー: {ex.Message}");
    }
}
// --- ここまで ---
#endif

#if DEBUG
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Dapperを使って、DBの現在時刻を聞いてみる
        using var connection = new Npgsql.NpgsqlConnection(connectionString);
        var newDefect = new DefectApp.Models.Defect
        {
            ProductName = "Test ProductA",
            DefectType = "表面キズ",
            CauseAnalysis = "AI分析待ち...",
            CreatedAt = DateTime.UtcNow
        };

        var sql = "INSERT INTO defects (product_name, defect_type, cause_analysis, created_at) VALUES (@ProductName, @DefectType, @CauseAnalysis, @CreatedAt) RETURNING id;";
        connection.Execute(sql, newDefect);

    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ DB接続エラー: {ex.Message}");
    }
}
#endif

app.Run();
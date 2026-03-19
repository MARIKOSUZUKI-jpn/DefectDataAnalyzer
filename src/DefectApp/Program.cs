using Dapper; // ← これを一番上に追加！
using Npgsql; // ← これも必要です

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// appsettings.json から接続文字列を読み込む(2026.03.19 Add)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var app = builder.Build();

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
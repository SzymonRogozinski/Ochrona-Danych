using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BankAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitBankAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CryptedInfo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoginLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HowManyTrials = table.Column<int>(type: "int", nullable: false),
                    PasswordChanged = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<int>(type: "int", nullable: false),
                    Sender = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountNumber", "CryptedInfo", "UserName" },
                values: new object[,]
                {
                    { 1, "143601139776", "pO0nXCjQW+1xQN6hkxPdZ3RvQzGvbcP51a207wGO21DxkFW9A5cMeZTRzdwjhO68KFCuZHhKzOjxFB23PmRS+KptH4751sUJCYoqOcSR7sMG+xnTzy+anC8mp8QbgUT2nMhehcdeuRcU+YjdEdfI1Rf/f5MrE8ZFyZO0MfsLI7mkSRrmG+gskGFsxE+YD+NzNlyNmvS/JH2BA80N7b3mxY1FGtO1Ae7v2vwsU6gneVLsE+1jg+PGOZ2x7ZZa2hwcRVVdIEh4EoYgUvPsNX/QbMcmio7B2t9dQ927I3y7p9OW7NOZ1qU6l2uou4P7E4aJi0OxWrBlr7t1TynnIgjRuSAJS0N2qRyndmT5uBQggh1fIS8+3zrdYpQe5ZKfHhV0M65zToMx5NNKU9S1znDCHXpd6rFsJUmxuJcpJScAr4LDirNk1tFbOqdVGfGR96OFB3uvmOP81A00S/qEmfbbfJtANnhFzE+5gwvSKvQIe5YcA034YgqS6unyU2oFFMCegkt0CpJd04w0SqdDSzJUGUm6QuXHdakEIdHx0JW787PY3NCP8lSvdwkfzy82QQbRnoo1KtYZneneOTClMnWdbc8EFzBgzvuCKVSpIUBKicTIj4hmKEMRpmWJg2qJwhBgt5rNN4rmZyn6PwoQQJgLg1ECStn27sSYTVWi5wKuX699HQNxCcTHYfYusIxUOI5/K5DN9vGdSfmyT4qPSMbL3WfL4lpK6cFvkk6Ql9dFeCGCfr3+093NPiRaqlej5TGyIIdmuaeuBbJ72deqvK41KCUdmnAuzeTrIgXEcbsX+BFByCPi/6Wb7AbSGojeWafB/huxFVUfnn9vW8M49tn5ajoDavnJvgRe/XZKV7fBQlO2lbeKoXYWLd0tTVXq3ZZz", "BasiaK6" },
                    { 2, "298456755675", "fuUakrWUQaiY60G8GfoCa/sTS70eFQgYzYEoxINAWlQtYG1/vIwS7eYbpouqTLZseb3rjUrvmgqWwZ2vzPaIQXEKs8boSwS7XnMrgLRILzQaV+gPO8iBySHS1MkADbKY1kCASaTub1SpZ8jfMRVgmGKxKTfaXjB4IySJmHZvwQx06SkExoeQKH4JaDlm6nUdWROkE0Cl4B2jyYxl4b+XX6jVXzYFy6jHPeUh2qy27PFEaytRM6TRx9FwIW98iDwF9QwAgLgC0JFDIjrdDifBymsrXHWHhCJlsfBO/79fbBPFbvKKgT1bxBpx3zzNAcihFg6Xo19Ny8KaGA4sjbcBfdXAVQfFQRwpmXs1zoGwWMsejtHd8Useea8llPLOJ2us4cSbyndSiRBuPCHNBeP1yNiH8mtHmKIxYIZm8V7Hm2Dx0yWDptrdBTwR6mAGY7xu9qigkT/+SryeD0OMjvQ4hJE85Qo4b3vcNAjdKa+PqSmbI5VTOS9GHRdAvs/p+3hfKn9KweIiN1IukgCcQheh3EthJyc4TEf6mSd7kPzJO+HYmO6CAPO8vXc3myexn9hHIw83eY28txod2+gCmkj4F9oQG/txT4CKcW29i1hkCZZQWakMnWr3jhpMyWjBBJlzOiPbGwMFlVeggzHkbemd+ravPw0D5DKdZWIydEFIUcdCAXUK5Dm4JclElgouW8jUW2AXqmnF6/LLcObs01BwBGmK38RZh3REmL2Srw5HskJcMIGzRRsGyw4TiQYyd2jizYphdWZnGFXpdgngpEOb4PfH+JsYhA3Yt0CNbklMISiGSq+W0ky0DCp0w4ZTJumT+VNIfKl0fqulBkjbPCS09ANxAfXXzKvUQeoYbU+5qCoDLA4MqfeSM77E5v/wxI9AxX7DLQ25i2ao4A005pzoOA==", "Konon111" },
                    { 3, "609044535706", "t7HikjhObkgt0pWMUe/y4lDoZTO6JpPepsG7kz4REZqqsV7eBfHnfNi4KW4jq/nMYXsKzr4yoiLvGK8Wl/Z7L//0vSQAXLzLFnBKheS2qva4Vvx1gTIcHPo5TSlZ1iydbJnNwI6MoB+4YFs8k3S38AOYGpIUAGFXroiswFZlX4nos04N6aY4fII5t4kOdjUAPT1uuPvXxg1KNd3SLuWAKJYWoOHs3h59UO4OcWslqRNK7S/fumgEEin9BKKReEYoVHpOGcuwE8BfrAj1FCNF3K6gIm5nmM3UrlsN9Gx/Sfv31+kqrpkD5BVdhb9cKZT7kdL7QvzlEhI0c2BTSYoDdu/7En8oB9PwKf87qcIXMV301oLiq51MUK1t68OIdmmF9nGleR3CFwv6VkfcLTsLJcVYHss2mzug16Udgjr4uQhrZuRWJvBSFauCHvNbz6BFBvfTm+MsyaW5meWnrVhUfrzVTTfnAZpGlRWFSypDS0fDuedwQxs/XgU0esWbFDX3BstpltTjCrHOzN6N5oB43v9jB6Th8qQrtXZBfQKKGM5mvsmWxsm0pgXtPpn78xpA3b9WDbd0dwMmQUYtUlWIHctKEOlhiSi010JHWW6aHLPWN+yD8RM/aOMgCIhIV8LXT68fJqNYm7fYDH8ja2YNRVLSoTxFqx0EWYmvT/EbjG6lkznitI/1LJOpO7YlFZEyGX44J6iN1xqVgRTO3fS+3Tu9rvVj1Xewgi6AEd8Pbef/eucD5k5B1Tob6FEKIX4Wb0PqKfnF6QKOBhbCt/m2LuI9FtZWz1VMLwlUBNgNclPzY2LO/83IgQYOapgys29lnptQ7W9KQy+UCGED6zG4/Pd/Yv1TvCS5cR445gZS31n5ql9MCouo4GtN6LAeF/q0", "Administrator" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "LoginLogs");

            migrationBuilder.DropTable(
                name: "Transfers");
        }
    }
}

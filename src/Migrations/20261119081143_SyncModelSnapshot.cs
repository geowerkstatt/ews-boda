using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EWS.Migrations;

/// <summary>
/// Only syncs the model snapshot to the current model; no DDL is emitted.
/// The database keeps <c>new_usr</c> and <c>new_date</c> as NOT NULL.
/// </summary>
public partial class SyncModelSnapshot : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}

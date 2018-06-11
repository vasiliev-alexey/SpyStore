using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpyStore.DAL.Migrations
{
    public partial class SP_SQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"CREATE FUNCTION store.purchaseitemsincart(p_customerid integer DEFAULT 0, OUT p_orderid integer)" +
                      "  RETURNS integer  " +
                      "LANGUAGE plpgsql " +
                      "AS $$ " +
                      "BEGIN " +
                      "  INSERT INTO store.orders (\"CustomerId\", \"OrderDate\", \"ShipDate\")  " +
                      "       VALUES (p_customerId, date_trunc('dd.MM.yyyy', now()), date_trunc('dd.MM.yyyy', now())) " +
                      "      RETURNING id " +
                      "     INTO p_orderId; " +
                      "            BEGIN " +
                      "  START TRANSACTION; " +
                      "   INSERT INTO Store.order_details(\"OrderId\", \"ProductId\", \"Quantity\", \"UnitCost\") " +
                      "   SELECT " +
                      "   p_orderId,  " +
                      "     \"ProductId\",  " +
                      "    \"Quantity\", " +
                      "             p.CurrentPrice " +
                      "  FROM Store.shopping_cart_records scr " +
                      "   INNER JOIN Store.products p ON p.\"Id\" = scr.\"ProductId\"" +
                      " WHERE \"CustomerId\" = p_customerId;" +
                      "          DELETE FROM Store.shopping_cart_records" +
                      "             WHERE \"CustomerId\" = p_customerId;" +
                      "    COMMIT;" +
                      " EXCEPTION WHEN OTHERS" +
                      "  THEN" +
                      "      ROLLBACK;" +
                      "   p_orderId = -1;" +
                      "   END;" +
                      "   END;" +
                      "       $$;" +
                      ";";
Console.WriteLine("execute sp 1");
            migrationBuilder.Sql(sql);


            sql = @" CREATE or replace FUNCTION store.getordertotal(p_orderid integer) " +
                  "RETURNS numeric " +
                  "LANGUAGE plpgsql " +
                  "AS $$ " +
                  "DECLARE " +
                  "  Result NUMERIC; " +
                  "BEGIN " +
                  " SELECT SUM(\"Quantity\" * \"UnitCost\")" +
                  "       INTO Result" +
                  "      FROM Store.order_details" +
                  "          WHERE \"OrderId\" = p_orderId; " +
                  "      RETURN Result;" +
                  "      END;" +
                  "         $$;" +
                  "      ";
            Console.WriteLine("execute sp 2");
            migrationBuilder.Sql(sql);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION store.getordertotal(INTEGER);");
            migrationBuilder.Sql("DROP FUNCTION store.purchaseitemsincart(INTEGER);");
        }
    }
}
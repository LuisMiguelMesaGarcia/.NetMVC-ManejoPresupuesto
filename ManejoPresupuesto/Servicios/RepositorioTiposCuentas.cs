﻿using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace ManejoPresupuesto.Servicios
{
    public class RepositorioTiposCuentas:IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configutarion)
        {
            connectionString = configutarion.GetConnectionString("DefaultConnection");
        }

       

        public async Task Crear(TipoCuenta tipoCuenta) 
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar", 
                                                new {usuarioId = tipoCuenta.UsuarioId, nombre=tipoCuenta.Nombre},
                                                commandType: System.Data.CommandType.StoredProcedure);

            tipoCuenta.id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                        @"SELECT 1 FROM TiposCuentas
                                        WHERE Nombre=@Nombre AND UsuarioId=@UsuarioId;",
                                        new {nombre, usuarioId});
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId) 
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT id,Nombre,Orden
                                                          FROM TiposCuentas
                                                          WHERE UsuarioId = @UsuarioId
                                                          ORDER BY Orden", new { usuarioId });
        }

        public async Task Actualizar(TipoCuenta tipoCuenta) 
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas 
                                                    SET Nombre = @Nombre
                                                    WHERE id = @id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId) 
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT id,Nombre,ORDEN
                                                                    FROM TiposCuentas
                                                                    WHERE id = @id AND UsuarioId = @UsuarioId", new { id, usuarioId });
        }

        public async Task Borrar(int id) 
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TiposCuentas WHERE Id = @Id", new { id } );
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados) 
        {
            var query = @"UPDATE TiposCuentas SET Orden = @Orden Where id = @id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }
    }
}
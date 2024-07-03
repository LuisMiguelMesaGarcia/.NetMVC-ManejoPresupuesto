using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public class RepositorioCategorias:IRepositorioCategorias
    {
        private readonly string connectionString;
        public RepositorioCategorias(IConfiguration configuration )
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categoria categoria) 
        {
            using var connection = new SqlConnection( connectionString );
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categorias(Nombre,TipoOperacionId, UsuarioId)
                                                            VALUES(@Nombre,@TipoOperacionId,@UsuarioId);
                                                            SELECT SCOPE_IDENTITY();",categoria);
            categoria.id = id;
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId) 
        {
            using var connection = new SqlConnection ( connectionString );
            return await connection.QueryAsync<Categoria>(@"SELECT * FROM Categorias WHERE UsuarioId = @usuarioId", new {usuarioId});
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(@"SELECT * FROM Categorias 
                                                            WHERE UsuarioId = @usuarioId AND TipoOperacionId = @tipoOperacionId",
                                                            new { usuarioId, tipoOperacionId });
        }

        public async Task<Categoria> ObtenerPorId(int id, int usuarioId) 
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(@"Select * FROM Categorias
                                                                        WHERE id = @id AND UsuarioId = @UsuarioId", new {id,usuarioId} );
        }

        public async Task Actualizar(Categoria categoria) 
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Categorias
                                            SET Nombre= @Nombre, TipoOperacionId = @TipoOperacionId
                                            WHERE id= @id",categoria);
        }

        public async Task Borrar(int id) 
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE Categorias WHERE id = @id", new { id });
        }


    }
}

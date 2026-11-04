
using System.Data;
using System.Data.SqlClient;
using MiWebAPI.Models;

namespace MiWebAPI.Data
{
    public class DocenteData
    {
        private readonly string conexion;

        public DocenteData(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("CadenaSQL")!;
        }

        // LISTAR DOCENTES
        public async Task<List<Docente>> Lista()
        {
            List<Docente> lista = new List<Docente>();

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                SqlCommand cmd = new SqlCommand("sp_listaDocentes", con);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Docente
                        {
                            IdDocente = Convert.ToInt32(reader["IdDocente"]),
                            NombreCompleto = reader["NombreCompleto"].ToString(),
                            Correo = reader["Correo"].ToString(),
                            Tipo = reader["Tipo"].ToString(),
                            HorasContratadas = reader["HorasContratadas"] == DBNull.Value
                                ? null : Convert.ToDecimal(reader["HorasContratadas"]),
                            Disponibilidad = reader["Disponibilidad"] == DBNull.Value
                                ? null : reader["Disponibilidad"].ToString(),
                            Observaciones = reader["Observaciones"] == DBNull.Value
                                ? null : reader["Observaciones"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        // OBTENER DOCENTE POR ID
        public async Task<Docente> Obtener(int id)
        {
            Docente objeto = new Docente();

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                SqlCommand cmd = new SqlCommand("sp_obtenerDocente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdDocente", id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        objeto = new Docente
                        {
                            IdDocente = Convert.ToInt32(reader["IdDocente"]),
                            NombreCompleto = reader["NombreCompleto"].ToString(),
                            Correo = reader["Correo"].ToString(),
                            Tipo = reader["Tipo"].ToString(),
                            HorasContratadas = reader["HorasContratadas"] == DBNull.Value
                                ? null : Convert.ToDecimal(reader["HorasContratadas"]),
                            Disponibilidad = reader["Disponibilidad"] == DBNull.Value
                                ? null : reader["Disponibilidad"].ToString(),
                            Observaciones = reader["Observaciones"] == DBNull.Value
                                ? null : reader["Observaciones"].ToString()
                        };
                    }
                }
            }

            return objeto;
        }

        // CREAR DOCENTE
        public async Task<RespuestaOperacion> Crear(Docente objeto)
        {
            RespuestaOperacion respuesta = new RespuestaOperacion();

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                SqlCommand cmd = new SqlCommand("sp_crearDocente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@NombreCompleto", objeto.NombreCompleto!);
                cmd.Parameters.AddWithValue("@Correo", objeto.Correo!);
                cmd.Parameters.AddWithValue("@Tipo", objeto.Tipo!);
                cmd.Parameters.AddWithValue("@HorasContratadas",
                    (object?)objeto.HorasContratadas ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Disponibilidad",
                    (object?)objeto.Disponibilidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones",
                    (object?)objeto.Observaciones ?? DBNull.Value);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        respuesta.NumeroError = Convert.ToInt32(reader["NumeroError"]);
                        respuesta.Respuesta = reader["Respuesta"].ToString()!;
                    }
                }
            }

            return respuesta;
        }

        // EDITAR DOCENTE
        public async Task<RespuestaOperacion> Editar(Docente objeto)
        {
            RespuestaOperacion respuesta = new RespuestaOperacion();

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                SqlCommand cmd = new SqlCommand("sp_editarDocente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdDocente", objeto.IdDocente);
                cmd.Parameters.AddWithValue("@NombreCompleto", objeto.NombreCompleto!);
                cmd.Parameters.AddWithValue("@Correo", objeto.Correo!);
                cmd.Parameters.AddWithValue("@Tipo", objeto.Tipo!);
                cmd.Parameters.AddWithValue("@HorasContratadas",
                    (object?)objeto.HorasContratadas ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Disponibilidad",
                    (object?)objeto.Disponibilidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones",
                    (object?)objeto.Observaciones ?? DBNull.Value);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        respuesta.NumeroError = Convert.ToInt32(reader["NumeroError"]);
                        respuesta.Respuesta = reader["Respuesta"].ToString()!;
                    }
                }
            }

            return respuesta;
        }

        // ELIMINAR DOCENTE
        public async Task<RespuestaOperacion> Eliminar(int id)
        {
            RespuestaOperacion respuesta = new RespuestaOperacion();

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                SqlCommand cmd = new SqlCommand("sp_eliminarDocente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdDocente", id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        respuesta.NumeroError = Convert.ToInt32(reader["NumeroError"]);
                        respuesta.Respuesta = reader["Respuesta"].ToString()!;
                    }
                }
            }

            return respuesta;
        }
    }
}

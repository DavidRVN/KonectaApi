using KonectaApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace KonectaApi.Controllers
{
    public class ColaboradoresController : ApiController
    {
        string connString = ConfigurationManager.ConnectionStrings["KonectaDB"].ConnectionString;

        [HttpPost]
        [Route("api/RegistrarColaborador")]
        public IHttpActionResult RegistrarColaborador(Colaborador colaborador)
        {
            if (colaborador == null)
                return BadRequest("Datos inválidos.");

            if (string.IsNullOrEmpty(colaborador.NumeroIdentificacion))
                return BadRequest("El número de identificación es obligatorio.");

            if (colaborador.Salario <= 0)
                return BadRequest("El salario debe ser mayor a 0.");

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Validar documento duplicado
                    string checkDoc = "SELECT COUNT(*) FROM Colaboradores WHERE NumeroIdentificacion=@doc";
                    SqlCommand cmdDoc = new SqlCommand(checkDoc, conn);
                    cmdDoc.Parameters.AddWithValue("@doc", colaborador.NumeroIdentificacion);
                    if ((int)cmdDoc.ExecuteScalar() > 0)
                        return BadRequest("El número de identificación ya está registrado.");

                    // Validar email duplicado
                    string checkEmail = "SELECT COUNT(*) FROM Colaboradores WHERE Email=@mail";
                    SqlCommand cmdEmail = new SqlCommand(checkEmail, conn);
                    cmdEmail.Parameters.AddWithValue("@mail", colaborador.Email);
                    if ((int)cmdEmail.ExecuteScalar() > 0)
                        return BadRequest("El correo electrónico ya está registrado.");

                    // Validar área existente
                    string checkArea = "SELECT COUNT(*) FROM Areas WHERE IdArea=@idArea";
                    SqlCommand cmdArea = new SqlCommand(checkArea, conn);
                    cmdArea.Parameters.AddWithValue("@idArea", colaborador.IdArea);
                    if ((int)cmdArea.ExecuteScalar() == 0)
                        return BadRequest("El área especificada no existe.");

                    // Insertar colaborador
                    string query = @"INSERT INTO Colaboradores
                            (NumeroIdentificacion, Nombres, Apellidos, Direccion, Email, Telefono, Salario, IdArea, FechaIngreso, Sexo)
                             VALUES
                            (@NumeroIdentificacion, @Nombres, @Apellidos, @Direccion, @Email, @Telefono, @Salario, @IdArea, @FechaIngreso, @Sexo)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NumeroIdentificacion", colaborador.NumeroIdentificacion);
                    cmd.Parameters.AddWithValue("@Nombres", colaborador.Nombres);
                    cmd.Parameters.AddWithValue("@Apellidos", colaborador.Apellidos);
                    cmd.Parameters.AddWithValue("@Direccion", colaborador.Direccion);
                    cmd.Parameters.AddWithValue("@Email", colaborador.Email);
                    cmd.Parameters.AddWithValue("@Telefono", colaborador.Telefono);
                    cmd.Parameters.AddWithValue("@Salario", colaborador.Salario);
                    cmd.Parameters.AddWithValue("@IdArea", colaborador.IdArea);
                    cmd.Parameters.AddWithValue("@FechaIngreso", colaborador.FechaIngreso);
                    cmd.Parameters.AddWithValue("@Sexo", colaborador.Sexo);

                    cmd.ExecuteNonQuery();
                }

                return Ok("Colaborador registrado correctamente.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("api/ConsultarColaboradorPorIdentificacion/{id}")]
        public IHttpActionResult ConsultarColaboradorPorIdentificacion(string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = @"SELECT C.NumeroIdentificacion, C.Nombres, C.Apellidos, C.Direccion, C.Email, 
                                    C.Telefono, C.Salario, C.IdArea, A.NombreArea, C.FechaIngreso, C.Sexo
                             FROM Colaboradores C
                             INNER JOIN Areas A ON C.IdArea = A.IdArea
                             WHERE NumeroIdentificacion = @id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var result = new
                        {
                            NumeroIdentificacion = reader["NumeroIdentificacion"].ToString(),
                            Nombres = reader["Nombres"].ToString(),
                            Apellidos = reader["Apellidos"].ToString(),
                            Direccion = reader["Direccion"].ToString(),
                            Email = reader["Email"].ToString(),
                            Telefono = reader["Telefono"].ToString(),
                            Salario = Convert.ToDecimal(reader["Salario"]),
                            IdArea = Convert.ToInt32(reader["IdArea"]),
                            NombreArea = reader["NombreArea"].ToString(),
                            Area = reader["NombreArea"].ToString(),
                            FechaIngreso = Convert.ToDateTime(reader["FechaIngreso"]).ToString("yyyy-MM-dd"),
                            Sexo = reader["Sexo"].ToString()
                        };
                        return Ok(result);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPut]
        [Route("api/ActualizarColaborador/{id}")]
        public IHttpActionResult ActualizarColaborador(string id, Colaborador colaborador)
        {
            if (colaborador == null || id != colaborador.NumeroIdentificacion)
                return BadRequest("Datos inválidos.");

            if (colaborador.Salario <= 0)
                return BadRequest("El salario debe ser mayor a 0.");

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Validar existencia del colaborador
                    string checkExist = "SELECT COUNT(*) FROM Colaboradores WHERE NumeroIdentificacion=@doc";
                    SqlCommand cmdExist = new SqlCommand(checkExist, conn);
                    cmdExist.Parameters.AddWithValue("@doc", colaborador.NumeroIdentificacion);
                    if ((int)cmdExist.ExecuteScalar() == 0)
                        return NotFound();

                    // Validar email duplicado (excepto si es del mismo colaborador)
                    string checkEmail = "SELECT COUNT(*) FROM Colaboradores WHERE Email=@mail AND NumeroIdentificacion<>@doc";
                    SqlCommand cmdEmail = new SqlCommand(checkEmail, conn);
                    cmdEmail.Parameters.AddWithValue("@mail", colaborador.Email);
                    cmdEmail.Parameters.AddWithValue("@doc", colaborador.NumeroIdentificacion);
                    if ((int)cmdEmail.ExecuteScalar() > 0)
                        return BadRequest("El correo electrónico ya está registrado por otro colaborador.");

                    // Validar área existente
                    string checkArea = "SELECT COUNT(*) FROM Areas WHERE IdArea=@idArea";
                    SqlCommand cmdArea = new SqlCommand(checkArea, conn);
                    cmdArea.Parameters.AddWithValue("@idArea", colaborador.IdArea);
                    if ((int)cmdArea.ExecuteScalar() == 0)
                        return BadRequest("El área especificada no existe.");

                    // Actualizar colaborador
                    string query = @"UPDATE Colaboradores
                             SET Nombres=@Nombres, Apellidos=@Apellidos, Direccion=@Direccion, Email=@Email, 
                                 Telefono=@Telefono, Salario=@Salario, IdArea=@IdArea, FechaIngreso=@FechaIngreso, Sexo=@Sexo
                             WHERE NumeroIdentificacion=@NumeroIdentificacion";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NumeroIdentificacion", colaborador.NumeroIdentificacion);
                    cmd.Parameters.AddWithValue("@Nombres", colaborador.Nombres);
                    cmd.Parameters.AddWithValue("@Apellidos", colaborador.Apellidos);
                    cmd.Parameters.AddWithValue("@Direccion", colaborador.Direccion);
                    cmd.Parameters.AddWithValue("@Email", colaborador.Email);
                    cmd.Parameters.AddWithValue("@Telefono", colaborador.Telefono);
                    cmd.Parameters.AddWithValue("@Salario", colaborador.Salario);
                    cmd.Parameters.AddWithValue("@IdArea", colaborador.IdArea);
                    cmd.Parameters.AddWithValue("@FechaIngreso", colaborador.FechaIngreso);
                    cmd.Parameters.AddWithValue("@Sexo", colaborador.Sexo);

                    cmd.ExecuteNonQuery();
                }

                return Ok("Colaborador actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        // Eliminar colaborador
        [HttpDelete]
        [Route("api/EliminarColaborador/{id}")]
        public IHttpActionResult EliminarColaborador(string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "DELETE FROM Colaboradores WHERE NumeroIdentificacion=@id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                        return NotFound();
                }

                return Ok("Colaborador eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }
}
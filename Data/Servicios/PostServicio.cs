using System.Data;
using System.Data.SqlClient;
using NetBlog.Data.Interfaces.IRepositorio;
using NetBlog.Models;

namespace NetBlog.Data.Servicios;

public class PostServicio : IRepositorioBase
{
    private readonly Contexto _contexto;

    public PostServicio(Contexto contexto)
    {
        _contexto = contexto;
    }

    public void UpdatePost(Post post)
    {
        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();
            using (var command = new SqlCommand("ActualizarPost", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PostId", post.PostId);
                command.Parameters.AddWithValue("@Titulo", post.Titulo);
                command.Parameters.AddWithValue("@Contenido", post.Contenido);
                command.Parameters.AddWithValue("@CategoriaId", post.CategoriaId);
                command.ExecuteNonQuery();
            }
        }
    }

    public void InsertPost(Post post)
    {
        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();
            using (var command = new SqlCommand("AgregarPost", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Titulo", post.Titulo);
                command.Parameters.AddWithValue("@Contenido", post.Contenido);
                command.Parameters.AddWithValue("@CategoriaId", post.CategoriaId);
                DateTime fc = DateTime.UtcNow;
                command.Parameters.AddWithValue("@FechaCreacion", fc);
                command.ExecuteNonQuery();
            }
        }
    }

    public Post ObtenerPostId(int id)
    {
        var post = new Post();
        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();

            using (var command = new SqlCommand("ObtenerPostPorId", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PostId", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        post = new Post
                        {
                            PostId = (int)reader["PostId"],
                            Titulo = (string)reader["Titulo"],
                            Contenido = reader["Contenido"].ToString(),
                            CategoriaId = (int)reader["CategoriaId"],
                            FechaCreacion = (DateTime)reader["FechaCreacion"]
                        };
                    }
                    reader.Close();
                }
            }
        }
        return post;
    }

    public void AgregarComentario(int postId, string comentario, int userId, int? comentarioPadreId)
    {
        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            //connection.Open();

            using (var command = new SqlCommand("AgregarComentario", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Contenido", SqlDbType.VarChar).Value = comentario;
                command.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value =
                    DateTime.UtcNow;
                command.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@PostId", SqlDbType.Int).Value = postId;
                command.Parameters.Add("@ComentarioPadreId", SqlDbType.Int).Value =
                    comentarioPadreId;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void DeletePost(int id)
    {
        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();

            using (var command = new SqlCommand("EliminarPost", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PostId", id);
                command.ExecuteNonQuery();
            }
        }
    }

    public List<Post> ObtenerTodos()
    {
        var posts = new List<Post>();

        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();

            using (var command = new SqlCommand("ObtenerTodosLosPost", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var post = new Post
                        {
                            PostId = (int)reader["PostId"],
                            Titulo = (string)reader["Titulo"],
                            Contenido = reader["Contenido"].ToString(),
                            CategoriaId = (int)reader["CategoriaId"],
                            FechaCreacion = (DateTime)reader["FechaCreacion"]
                        };

                        posts.Add(post);
                    }
                    reader.Close();
                }
            }
        }
        return posts;
    }

    public List<Post> ObtenerPostCategoria(int id)
    {
        var posts = new List<Post>();

        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();

            using (var command = new SqlCommand("ObtenerPostCategoria", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CategoriaId", id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var post = new Post
                        {
                            PostId = (int)reader["PostId"],
                            Titulo = (string)reader["Titulo"],
                            Contenido = reader["Contenido"].ToString(),
                            CategoriaId = (int)reader["CategoriaId"],
                            FechaCreacion = (DateTime)reader["FechaCreacion"]
                        };

                        posts.Add(post);
                    }
                    reader.Close();
                }
            }
        }
        return posts;
    }

    public List<Post> ObtenerPostTitulo(string titulo)
    {
        var posts = new List<Post>();

        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();

            using (var command = new SqlCommand("ObtenerPostTitulo", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Titulo", titulo);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        posts.Add(new Post
                        {
                            PostId = (int)reader["PostId"],
                            Titulo = (string)reader["Titulo"],
                            Contenido = reader["Contenido"].ToString(),
                            CategoriaId = (int)reader["CategoriaId"],
                            FechaCreacion = (DateTime)reader["FechaCreacion"]
                        });

                        //posts.Add(post);
                    }
                    reader.Close();
                }
            }
        }
        return posts;
    }

    public List<Comentario> ObtenerComentariosPorPostId(int id)
    {
        //ObtenerComentariosPorPostId
        var comments = new List<Comentario>();

        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();

            using (var command = new SqlCommand("ObtenerComentariosPorPostId", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PostId", id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var comment = new Comentario
                        {
                            ComentarioId = (int)reader["ComentarioId"],
                            Contenido = reader["Contenido"].ToString(),
                            FechaCreacion = (DateTime)reader["FechaCreacion"],
                            UsuarioId = (int)reader["UsuarioId"],
                            PostId = (int)reader["PostId"],
                            NombreUsuario = reader["NombreUsuario"].ToString(),
                            //ComentarioPadreId = (int)reader["ComentarioPadreId"],
                        };
                        comments.Add(comment);
                    }
                    reader.Close();
                }
            }
        }
        return comments;
    }

    public List<Comentario> ObtenerComentariosHijos(List<Comentario> comments)
    {
        //var comments = new List<Comentario>();

        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();
            foreach (var comment in comments)
            {
                using (
                    var command = new SqlCommand(
                        "ObtenerComentariosHijosPorComentarioId",
                        connection
                    )
                )
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ComentarioId", comment.ComentarioId);

                    using (var reader = command.ExecuteReader())
                    {
                        var comentariosHijos = new List<Comentario>();
                        while (reader.Read())
                        {
                            var comentarioHijo = new Comentario
                            {
                                ComentarioId = (int)reader["ComentarioId"],
                                Contenido = reader["Contenido"].ToString(),
                                FechaCreacion = (DateTime)reader["FechaCreacion"],
                                UsuarioId = (int)reader["UsuarioId"],
                                PostId = (int)reader["PostId"],
                                NombreUsuario = reader["NombreUsuario"].ToString(),
                                ComentarioPadreId = comment.ComentarioId
                            };
                            comentariosHijos.Add(comentarioHijo);
                        }
                        reader.Close();
                        comment.ComentariosHijos = comentariosHijos;
                    }
                }
            }
        }

        return comments;
    }

    public List<Comentario> ObtenerComentariosNietos(List<Comentario> comments)
    {
        //var comments = new List<Comentario>();

        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();
            foreach (var comment in comments)
            {
                if (comment.ComentariosHijos is not null)
                {
                    foreach (var comentarioHijo in comment.ComentariosHijos)
                    {
                        using (
                            var command = new SqlCommand(
                                "ObtenerComentariosHijosPorComentarioId",
                                connection
                            )
                        )
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue(
                                "@ComentarioId",
                                comentarioHijo.ComentarioId
                            );

                            using (var reader = command.ExecuteReader())
                            {
                                var comentariosNietos = new List<Comentario>();
                                while (reader.Read())
                                {
                                    var comentarioNieto = new Comentario
                                    {
                                        ComentarioId = (int)reader["ComentarioId"],
                                        Contenido = reader["Contenido"].ToString(),
                                        FechaCreacion = (DateTime)reader["FechaCreacion"],
                                        UsuarioId = (int)reader["UsuarioId"],
                                        PostId = (int)reader["PostId"],
                                        NombreUsuario = reader["NombreUsuario"].ToString(),
                                        ComentarioPadreId = comentarioHijo.ComentarioId,
                                        ComentarioAbuelo = comment.ComentarioId
                                    };
                                    comentarioNietos.Add(comentarioNieto);
                                }
                                reader.Close();
                                comentarioHijo.comentariosHijos = comentariosNietos;
                            }
                        }
                    }
                }
            }
        }

        return comments;
    }
}

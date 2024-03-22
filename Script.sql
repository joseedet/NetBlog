
CREATE TABLE Roles (
    RolId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    Nombre varchar(50) 
)
INSERT INTO Roles
    ( Nombre)
VALUES
    ('Administrador');
INSERT INTO Roles
    (Nombre)
VALUES
    ('Usuario');

------------------------------------------------------------------
CREATE PROCEDURE ListarRoles
AS BEGIN

SELECT * FROM Roles;

END

-----------------------------------------------------------------
CREATE TABLE Usuarios (UsuarioId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Nombre VARCHAR (50),Apellidos VARCHAR (50), Correo VARCHAR (100) UNIQUE,
Contrasenya VARCHAR(MAX),RolId INT, NombreUsuario VARCHAR (50) UNIQUE,
Estado BIT, Token VARCHAR(MAX), FechaExpiracion DATETIME
)
--------------------------------------------------------------------------

CREATE PROCEDURE RegistrarAusuario
@Nombre VARCHAR(50),
@Apellidos VARCHAR (50),
@Correo VARCHAR (100),
@Contrasenya VARCHAR (MAX),
@RolId INT=2,
@NombreUsuario VARCHAR (50) ,
@Estado BIT, 
@Token VARCHAR (MAX), 
@FechaExpiracion DATETIME
AS BEGIN
INSERT INTO Usuarios VALUES(@Nombre,@Apellidos,@Correo,@Contrasenya,@RolId,
@NombreUsuario,@Estado,@Token,@FechaExpiracion)
END
---------------------------------------------------------------------------

CREATE PROCEDURE ActivarCuenta
@Token VARCHAR(MAX),
@Fecha DATETIME

AS BEGIN

    DECLARE @Correo VARCHAR(100)
    DECLARE @FechaExpiracion DATETIME

    SET @Correo=(SELECT Correo from Usuarios WHERE Token=@Token)
    SET @Fecha=(SELECT FechaExpiracion FROM Usuarios WHERE Token=@Token)

    if @FechaExpiracion < @Fecha

    BEGIN
        UPDATE Usuarios SET Estado=1 WHERE Token=@Token
        UPDATE Usuarios SET Token=NULL WHERE Correo=@Correo
        SELECT 1 AS Resultado
    END
    ELSE
    BEGIN
        SELECT 0 as Resultado
    END
END
--------------------------------------------------------------------------

CREATE PROCEDURE ValidarUsuario
@Correo VARCHAR(100)
AS
BEGIN
SELECT * FROM Usuarios WHERE Correo=@Correo
END 

-----------------------------------------------------------------------

CREATE PROCEDURE ActualizarToken
@Correo VARCHAR(100),
@Fecha DATETIME,
@Token VARCHAR(MAX)

AS BEGIN

UPDATE Usuarios SET Token=@Token, FechaExpiracion=@Fecha WHERE Correo=@Correo 

END

------------------------------------------------------------------------------

CREATE PROCEDURE ActualizarPerfil
@UsuarioId INT,
@Nombre VARCHAR(50),
@Apellidos VARCHAR(59),
@Correo VARCHAR(50)

AS BEGIN

UPDATE Usuarios SET Nombre=@Nombre, Apellidos=@Apellidos, Correo=@Correo 
    WHERE UsuarioId=@UsuarioId

END
------------------------------------------------------------------------------

CREATE PROCEDURE ActualizarUsuario
    @UsuarioId INT,
    @Nombre VARCHAR(50),
    @Apellidos VARCHAR(59),
    @RolId INT,
    @Estado BIT

AS
BEGIN

    UPDATE Usuarios SET Nombre=@Nombre, Apellidos=@Apellidos,RolId=@RolId,Estado=@Estado 
    WHERE UsuarioId=@UsuarioId

END
-------------------------------------------------------------------------------------

CREATE PROCEDURE EliminarUsuario

@UsuarioId INT

AS BEGIN

DELETE FROM Usuarios WHERE UsuarioId=@UsuarioId

END

-------------------------------------------------------------------------------------

CREATE TABLE Categorias(
    CategoriaId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    Nombre VARCHAR(100) UNIQUE
)
--------------------------------------------------------------------------------------

CREATE PROCEDURE AgregarCategoria
@Nombre VARCHAR(100)
AS BEGIN
INSERT INTO Categtorias VALUES(@Nombre)
END

--------------------------------------------------------------------------------------

CREATE PROCEDURE ActualiarCategoria
@CategoriaId INT,
@Nombre VARCHAR (100)
AS BEGIN

UPDATE Categorias SET Nombre=@Nombre WHERE CategoriaId=@CategoriaId
END
--------------------------------------------------------------------------------------

CREATE PROCEDURE EliminarCategoria
@CategoriaId INT
AS BEGIN

DELETE FROM Categorias Where CategoriaId=@CategoriaId
END

--------------------------------------------------------------------------------------


CREATE TABLE Post(
    PostId INT IDENTITY(1,1) PRIMARY KEY,
    Titulo VARCHAR (500),
    Contenido VARCHAR(MAX),
    CategoriaId INT,
    FechaCreacion DATETIME

)
----------------------------------------------------------------------------------------

CREATE PROCEDURE AgregarPost
@Titulo VARCHAR(500),
@Contenido VARCHAR (MAX),
@CategoriaId INT,
@FechaCreacion DATETIME

AS BEGIN

INSERT INTO Post VALUES(@Titulo,@Contenido,@CategoriaId,@FechaCreacion)

END
--------------------------------------------------------------------------------

CREATE PROCEDURE ActualizarPost 
@PostId INT,
@Titulo VARCHAR (500),
@Contenido VARCHAR (MAX),
@CategoriaId INT,
@FechaCreacion DATETIME

AS BEGIN

UPDATE Post SET Titulo=@Titulo, Contenido=@Contenido,
    Categoria=@CategoriaId,FechaCreacion =@FechaCreacion

END
------------------------------------------------------------------------------

CREATE PROCEDURE ObtenerPostPorId
@PostId INT
AS BEGIN

SELECT * FROM Post WHERE PostId=@PostId

END
-----------------------------------------------------------------------------

CREATE PROCEDURE EliminarPost
    @PostId INT
AS
BEGIN

   DELETE
        Post
    WHERE PostId=@PostId

END
----------------------------------------------------------------------------

CREATE PROCEDURE ObtenerTodosLosPost
  
AS
BEGIN

    SELECT * FROM Post    

END

----------------------------------------------------------------------------

CREATE PROCEDURE ObtenerPostCategoria
@CategoriaId INT
AS
BEGIN

    SELECT * FROM Post WHERE CategoriaId = @CategoriaId 

END

---------------------------------------------------------------------------

CREATE PROCEDURE ObtenerPostTitulo
@Titulo VARCHAR (500)
AS
BEGIN
SELECT * FROM Post WHERE Titulo LIKE 
'%' + @Titulo + '%'
END

---------------------------------------------------------------------------

CREATE TABLE Comentarios(
    ComentarioId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    Contenido VARCHAR(MAX),
    FechaCreacion DATETIME,
    UsuarioId INT,
    PostId INT,
    ComentarioPadreId INT NULL,

    CONSTRAINT FK_Comentario_UsuarioId FOREIGN KEY (UsuarioId) REFERENCES 
    Usuarios(UsuarioId) ON DELETE CASCADE,

    CONSTRAINT FK_Comentario_PostId FOREIGN KEY (PostId) REFERENCES 
    Post(PostId) ON DELETE CASCADE,

    CONSTRAINT FK_Comentario_ComentarioPadreId FOREIGN KEY (ComentarioPadreId) REFERENCES 
    Comentarios(ComentarioId) ON DELETE NO ACTION
)
-----------------------------------------------------------------------------------------------
CREATE TRIGGER TR_EliminarComentariosHijos ON Comentarios
AFTER DELETE
AS
BEGIN
    DELETE FROM Comentario WHERE ComentarioPadreId IN (SELECT ComentarioId 
    FROM DELETED)
END

-----------------------------------------------------------------------------------------------

CREATE PROCEDURE ObtenerComentariosPorPostId
@PostId INT
AS
BEGIN
SELECT c.ComentarioId,c.Contenido,c.FechaCreacion,c.Usuario,c.PostId,u.NombreUsuario
FROM Comentario c
INNER JOIN Usuarios u ON u.UsuarioId=c.UsuarioId
WHERE c.PostId=@PostId AND c.ComentarioPadreId IS NULL
END
-------------------------------------------------------------------------------------------------

CREATE PROCEDURE ObtenerComentariosHijosPorComentarioId
@ComentarioId INT
AS
BEGIN
SELECT c.ComentarioId, c.Contenido, c.FechaCreacion, c.Usuario, c.PostId, u.NombreUsuario
FROM Comentario c
    INNER JOIN Usuarios u ON u.UsuarioId=c.UsuarioId
WHERE c.ComentarioPadreId=@ComentarioId
END

------------------------------------------------------------------------------------------------

CREATE PROCEDURE AgregarComentario
    @Contenido VARCHAR (MAX),
    @FechaCreacion DATETIME,
    @UsuarioId INT,
    @PostId INT,
    @ComentarioPadreId INT =NULL

AS BEGIN

    INSERT INTO Comentarios VALUES(@Contenido,@FechaCreacion,@UsuarioId,@PostId,@ComentarioPadreId)

END
-- SCRUM-33: Migração Prep_Time e Cook_Time de VARCHAR para INT (minutos totais)
-- Correr no SSMS na base de dados do projeto

-- Passo 1: Limpar dados existentes e converter colunas para INT
UPDATE Recipe SET Prep_Time = NULL, Cook_Time = NULL;

ALTER TABLE Recipe ALTER COLUMN Prep_Time INT NULL;
ALTER TABLE Recipe ALTER COLUMN Cook_Time INT NULL;

-- Passo 2: Atualizar a stored procedure spRecipe_Insert
-- (substituir NVARCHAR por INT nos parâmetros @Prep_Time e @Cook_Time)
-- Correr EXEC sp_helptext 'spRecipe_Insert' primeiro para ver o corpo atual
-- e depois fazer ALTER com a estrutura abaixo, mantendo o corpo intacto:

-- ALTER PROCEDURE spRecipe_Insert
--     @Id_User INT,
--     @NameRecipe NVARCHAR(255),
--     @Prep_Time INT,
--     @Cook_Time INT,
--     @Preparation NVARCHAR(MAX),
--     @Image VARBINARY(MAX),
--     @Recipe_Id_Out INT OUTPUT
-- AS
-- BEGIN
--     -- corpo igual ao original
-- END

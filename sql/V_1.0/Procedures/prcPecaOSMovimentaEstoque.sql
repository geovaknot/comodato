GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================  
-- Author:  Edgar Coutinho
-- Create date: 23/07/2021  
-- Description: Movimentação de estoque peça da OS
-- =============================================  
CREATE PROCEDURE [dbo].[prcPecaOSMovimentaEstoque]   
 @p_ID_OS     BIGINT,  
 @p_CD_PECA     VARCHAR(15),  
 @p_QT_PECA     NUMERIC(15,3),  
 @p_CD_TP_ESTOQUE_CLI_TEC CHAR(1),  
 @p_CD_TECNICO    VARCHAR(06),  
 @p_CD_CLIENTE    NUMERIC(06),  
 @p_nidUsuarioAtualizacao NUMERIC(18,0) = NULL,  
 @p_Mensagem     VARCHAR(8000) OUTPUT,
 @p_Permite_Movimentar_Estoque BIT OUTPUT,
 @p_Tipo_Movimentacao TINYINT
AS  
BEGIN  
  
 -- Declaração de Variáveis  
 DECLARE @cdsErrorMessage NVARCHAR(4000),  
   @nidErrorSeverity INT,  
   @nidErrorState  INT,  
   @nidLog    NUMERIC(18,0),  
  
   @ID_ESTOQUE BIGINT = NULL,  
   @ID_ESTOQUE_PECA BIGINT = NULL,  
   @QT_PECA_ATUAL NUMERIC(15,3) = NULL,  
   @CD_TP_MOVIMENTACAO CHAR(1) = NULL,  
   @DT_MOVIMENTACAO DATETIME = NULL,  
   @ID_ESTOQUE_MOVI BIGINT = NULL,  
   @IDENTIFICA_MOVIMENTACAO CHAR(1) = NULL

 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
  
 BEGIN TRY  
    
  -- Estoque Intermediário (Técnico)  
  IF (@p_CD_TP_ESTOQUE_CLI_TEC = 'T')  
   BEGIN  

    -- Busca o ESTOQUE (Header)  
    SELECT TOP 1 @ID_ESTOQUE = ISNULL(dbo.TbEstoque.ID_ESTOQUE, 0)  
      FROM dbo.TB_TECNICO   
     INNER JOIN dbo.tbEstoque  
        ON dbo.TB_TECNICO.ID_USUARIO = dbo.tbEstoque.ID_USU_RESPONSAVEL  
     WHERE dbo.TB_TECNICO.CD_TECNICO  = @p_CD_TECNICO -- através do técnico vincula os ID_USUARIOS no primeiro JOIN    
       AND dbo.tbEstoque.TP_ESTOQUE_TEC_3M = 'TEC'   -- forçar a consulta do estoque somente do TÉCNICO  
  
    -- Busca o ESTOQUE_PECA  
    SELECT @ID_ESTOQUE_PECA = ISNULL(dbo.tbEstoquePeca.ID_ESTOQUE_PECA, 0),  
		   @QT_PECA_ATUAL  = ISNULL(dbo.tbEstoquePeca.QT_PECA_ATUAL, 0)  
      FROM dbo.TB_TECNICO   
     INNER JOIN dbo.tbEstoque  
        ON dbo.TB_TECNICO.ID_USUARIO = dbo.tbEstoque.ID_USU_RESPONSAVEL  
     INNER JOIN dbo.tbEstoquePeca  
        ON dbo.tbEstoque.ID_ESTOQUE = dbo.tbEstoquePeca.ID_ESTOQUE  
     INNER JOIN dbo.TB_PECA  
        ON dbo.tbEstoquePeca.CD_PECA = dbo.TB_PECA.CD_PECA  
     WHERE dbo.TB_TECNICO.CD_TECNICO = @p_CD_TECNICO -- através do técnico vincula os ID_USUARIOS no primeiro JOIN    
       AND dbo.tbEstoque.TP_ESTOQUE_TEC_3M = 'TEC'   -- forçar a consulta do estoque somente do TÉCNICO  
       AND dbo.TB_PECA.CD_PECA = @p_CD_PECA  

   END   
    
  -- Estoque Cliente
  ELSE IF (@p_CD_TP_ESTOQUE_CLI_TEC = 'C')  
   BEGIN  

    -- Busca o ESTOQUE (Header)  
    SELECT TOP 1 @ID_ESTOQUE = ISNULL(dbo.TbEstoque.ID_ESTOQUE, 0)  
      FROM dbo.tbEstoque   
     WHERE dbo.tbEstoque .CD_CLIENTE = @p_CD_CLIENTE   
       AND dbo.tbEstoque.TP_ESTOQUE_TEC_3M = 'CLI'   -- forçar a consulta do estoque somente do CLIENTE  
  
    -- Busca o ESTOQUE_PECA  
    SELECT @ID_ESTOQUE_PECA = ISNULL(dbo.tbEstoquePeca.ID_ESTOQUE_PECA, 0),  
		   @QT_PECA_ATUAL = ISNULL(dbo.tbEstoquePeca.QT_PECA_ATUAL, 0)  
      FROM dbo.tbEstoque  
     INNER JOIN dbo.tbEstoquePeca  
        ON dbo.tbEstoque.ID_ESTOQUE = dbo.tbEstoquePeca.ID_ESTOQUE  
     INNER JOIN dbo.TB_PECA  
        ON dbo.tbEstoquePeca.CD_PECA = dbo.TB_PECA.CD_PECA  
     WHERE dbo.tbEstoque.CD_CLIENTE = @p_CD_CLIENTE   
       AND dbo.tbEstoque.TP_ESTOQUE_TEC_3M = 'CLI'   -- forçar a consulta do estoque somente do CLIENTE  
       AND dbo.TB_PECA.CD_PECA = @p_CD_PECA  

   END  
     
   SET @CD_TP_MOVIMENTACAO = (SELECT CD_TP_MOVIMENTACAO FROM dbo.tbTpEstoqueMovi WHERE DS_TP_MOVIMENTACAO = 'Utilização em atendimento');  
   SET @DT_MOVIMENTACAO = GETDATE();  
  
   -- Se não existir Estoque (Header) cancela gravação e retorna notificação  
   IF ISNULL(@ID_ESTOQUE, 0) = 0  
   BEGIN  
  
    IF (@p_CD_TP_ESTOQUE_CLI_TEC = 'T') -- Estoque Técnico
     BEGIN  
		SET @p_Mensagem = 'Técnico não possui <strong>Estoque Intermediário</strong> cadastrado!<br/>Favor criar o Estoque Intermediário ou utilizar o <strong>Estoque Cliente</strong> para continuar...';  
     END  
    ELSE IF (@p_CD_TP_ESTOQUE_CLI_TEC = 'C') -- Estoque Cliente
     BEGIN  
		SET @p_Mensagem = 'Cliente não possui <strong>Estoque</strong> cadastrado!<br/>Favor criar o Estoque ou utilizar o <strong>Estoque Intermediário</strong> para continuar...';  
     END  
  
    RETURN @p_Mensagem
   END            
   ELSE   
   BEGIN  
	 SET @p_Permite_Movimentar_Estoque = 1;

	 IF (@p_Tipo_Movimentacao = 1) -- Movimentação de ENTRADA
	 BEGIN
		SET @IDENTIFICA_MOVIMENTACAO = 'E';
		SET @QT_PECA_ATUAL = ISNULL(@QT_PECA_ATUAL, 0) + @p_QT_PECA;  
	 END

	 IF (@p_Tipo_Movimentacao = 2) -- Movimentação de SAIDA
	 BEGIN
		IF (@p_QT_PECA > ISNULL(@QT_PECA_ATUAL, 0))
		BEGIN
			SET @p_Permite_Movimentar_Estoque = 0;
			RETURN @p_Permite_Movimentar_Estoque
		END

		SET @IDENTIFICA_MOVIMENTACAO = 'S';
		SET @QT_PECA_ATUAL = ISNULL(@QT_PECA_ATUAL, 0) - @p_QT_PECA;  
	 END
  
    -- Se existir EstoquePeca, atualiza a quantidade  
    IF ISNULL(@ID_ESTOQUE_PECA, 0) > 0  
    BEGIN   

     EXEC dbo.prcEstoquePecaUpdate   
   		     @p_ID_ESTOQUE_PECA = @ID_ESTOQUE_PECA,  
			 @p_QT_PECA_ATUAL = @QT_PECA_ATUAL,  
			 @p_DT_ULT_MOVIM = @DT_MOVIMENTACAO,  
			 @p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao  
    
    END  
    -- Se não existir EstoquePeca, cria registro de EstoquePeca e atualiza a quantidade  
    ELSE   
    BEGIN  

     EXEC dbo.prcEstoquePecaInsert   
			 @p_CD_PECA = @p_CD_PECA,  
			 @p_QT_PECA_ATUAL = @QT_PECA_ATUAL,  
			 @p_QT_PECA_MIN = 0,  
			 @p_DT_ULT_MOVIM = @DT_MOVIMENTACAO,  
			 @p_ID_ESTOQUE = @ID_ESTOQUE,  
			 @p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,  
			 @p_ID_ESTOQUE_PECA = @ID_ESTOQUE_PECA  

    END  
    
    -- Criar registro de movimentação de estoque
    EXEC dbo.prcEstoqueMoviInsert   
			 @p_CD_TP_MOVIMENTACAO = @CD_TP_MOVIMENTACAO,  
			 @p_ID_OS = @p_ID_OS,  
			 @p_DT_MOVIMENTACAO = @DT_MOVIMENTACAO,  
			 @p_ID_ESTOQUE = @ID_ESTOQUE,  
			 @p_CD_PECA = @p_CD_PECA,  
			 @p_QT_PECA = @p_QT_PECA,  
			 @p_ID_USU_MOVI = @p_nidUsuarioAtualizacao,  
			 @p_ID_ESTOQUE_ORIGEM = NULL,  
			 @p_TP_ENTRADA_SAIDA = @IDENTIFICA_MOVIMENTACAO, 
			 @p_CD_CLIENTE = @p_CD_CLIENTE,  
			 @p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,  
			 @p_ID_ESTOQUE_MOVI = @ID_ESTOQUE_MOVI OUTPUT  
                
   END   
      
  
  SET @p_Mensagem = '';  
  
 END TRY  
  
 BEGIN CATCH  
  
  SELECT @cdsErrorMessage = ERROR_MESSAGE(),  
    @nidErrorSeverity = ERROR_SEVERITY(),  
    @nidErrorState  = ERROR_STATE();  
  
  --ROLLBACK TRANSACTION  
  
  -- Use RAISERROR inside the CATCH block to return error  
  -- information about the original error that caused  
  -- execution to jump to the CATCH block.  
  RAISERROR (@cdsErrorMessage, -- Message text.  
       @nidErrorSeverity, -- Severity.  
       @nidErrorState -- State.  
       )  
  
 END CATCH  
  
END  
  
  
  
GO

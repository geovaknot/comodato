GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================  
-- Author:  Alex Natalino  
-- Create date: 18/06/2018  
-- Description: Inclusão dos dados na tabela tbPecaOS 
-- =============================================  
ALTER PROCEDURE [dbo].[prcPecaOSInsert]   
 @p_ID_OS     BIGINT,  
 @p_CD_PECA     VARCHAR(15),  
 @p_QT_PECA     NUMERIC(15,3),  
 @p_CD_TP_ESTOQUE_CLI_TEC CHAR(1),  
 @p_CD_TECNICO    VARCHAR(06),  
 @p_CD_CLIENTE    NUMERIC(06),  
 @p_nidUsuarioAtualizacao NUMERIC(18,0) = NULL,  
 @p_ID_PECA_OS    BIGINT   OUTPUT,  
 @p_Mensagem     VARCHAR(8000) OUTPUT,
 @p_DS_OBSERVACAO     VARCHAR(MAX) = NULL,
 @p_TOKEN    		BIGINT			= NULL,
 @p_TOKEN_GERADO    BIGINT		    OUTPUT

AS  
BEGIN  
  
 -- Declaração de Variáveis  
 DECLARE @cdsErrorMessage NVARCHAR(4000),  
   @nidErrorSeverity INT,  
   @nidErrorState  INT,  
   @nidLog    NUMERIC(18,0),  
  
   @VL_VALOR_PECA  NUMERIC(18, 2)  = NULL,
   @TOKEN_REGISTRO_INCLUSAO BIGINT,
   @APLICACAO_ORIGEM_TOKEN BIGINT -- 1 = APP 2 = WEB
  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
  
 BEGIN TRY  
    
  
  BEGIN TRANSACTION  
  
  SELECT @VL_VALOR_PECA = VL_PECA FROM TB_PECA(NOLOCK) WHERE CD_PECA = @p_CD_PECA  
  
  -- Insere na tbPecaOS  
  INSERT INTO dbo.tbPecaOS  
    ( ID_OS,  
      CD_PECA,  
      QT_PECA,  
      VL_VALOR_PECA,  
      CD_TP_ESTOQUE_CLI_TEC,  
      nidUsuarioAtualizacao,  
      dtmDataHoraAtualizacao,
      DS_OBSERVACAO,
	  TOKEN
	  )  
  VALUES  
    ( @p_ID_OS,  
      @p_CD_PECA,  
      @p_QT_PECA,  
      @VL_VALOR_PECA,  
      @p_CD_TP_ESTOQUE_CLI_TEC,  
      @p_nidUsuarioAtualizacao,  
      GETDATE(),
      @p_DS_OBSERVACAO,
	  IIF(LEFT(@p_TOKEN, 1) = 1, @p_TOKEN, 0)
      )  
  
    SET @p_ID_PECA_OS = @@IDENTITY;  

	SET @APLICACAO_ORIGEM_TOKEN = LEFT(@p_TOKEN, 1);

	IF (@p_ID_PECA_OS > 0 AND @APLICACAO_ORIGEM_TOKEN = 2) -- 2 = ORIGEM Applicação WEB
	BEGIN
		SET @TOKEN_REGISTRO_INCLUSAO = CAST((CAST(@p_TOKEN AS nvarchar(MAX)) + CAST(@p_ID_PECA_OS AS nvarchar(MAX))) AS BIGINT);
			
		UPDATE dbo.tbPecaOS
			SET TOKEN = @TOKEN_REGISTRO_INCLUSAO
			WHERE ID_PECA_OS = @p_ID_PECA_OS

		SET @p_TOKEN_GERADO = @TOKEN_REGISTRO_INCLUSAO;
	END
	ELSE
	BEGIN
		SET @p_TOKEN_GERADO = @p_TOKEN;
	END
  
  EXECUTE dbo.prcLogGravar   
     @p_nidLog     = @nidLog,  
     @p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,  
     @p_ccdAcao     = 'I',  
     @p_cnmTabela    = 'tbPecaOS',  
     @p_nidPK     = @p_ID_PECA_OS,  
     @p_nidLogReturn    = @nidLog OUTPUT  
  
  SET @p_Mensagem = '';  
  
  COMMIT TRANSACTION  
   
 END TRY  
  
 BEGIN CATCH  
  
  SELECT @cdsErrorMessage = ERROR_MESSAGE(),  
    @nidErrorSeverity = ERROR_SEVERITY(),  
    @nidErrorState  = ERROR_STATE();  
  
  ROLLBACK TRANSACTION  
  
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
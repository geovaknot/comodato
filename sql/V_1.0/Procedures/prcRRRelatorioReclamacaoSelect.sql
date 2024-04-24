GO
/****** Object:  StoredProcedure [dbo].[prcRRRelatorioReclamacaoSelect]    Script Date: 15/06/2021 09:35:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
-- =============================================    
-- Create date: 24/01/2019    
-- Description: Seleção de dados na tabela     
--              TbRRRelatorioReclamacao     
-- =============================================    
ALTER PROCEDURE [dbo].[prcRRRelatorioReclamacaoSelect]    
 @p_ID_RELATORIO_RECLAMACAO BIGINT = NULL,     
 @p_ID_RR_STATUS BIGINT = NULL,     
 @p_CD_TECNICO VARCHAR(6)  = NULL,     
 @p_CD_CLIENTE VARCHAR(6)  = NULL,     
 @p_CD_ATIVO_FIXO VARCHAR(6)  = NULL,     
 @p_CD_PECA VARCHAR(15)  = NULL,     
 @p_CD_TIPO_ATENDIMENTO BIGINT = NULL,     
 @p_CD_TIPO_RECLAMACAO BIGINT = NULL,
 @p_ID_OS BIGINT = NULL
    
AS    
BEGIN    
    
 -- Declaração de Variáveis    
 DECLARE @cdsErrorMessage NVARCHAR(4000),    
   @nidErrorSeverity INT,    
   @nidErrorState  INT    
    
 -- SET NOCOUNT ON added to prevent extra result sets from    
 -- interfering with SELECT statements.    
 SET NOCOUNT ON;    
    
  BEGIN TRY    
      
    SELECT     
     TbRRRelatorioReclamacao.ID_RELATORIO_RECLAMACAO,    
     TbRRRelatorioReclamacao.ST_STATUS_RR,    
     TbRRRelatorioReclamacao.CD_TECNICO,    
     TbRRRelatorioReclamacao.CD_CLIENTE,    
     TbRRRelatorioReclamacao.CD_ATIVO_FIXO,    
     TbRRRelatorioReclamacao.CD_PECA,    
     TbRRRelatorioReclamacao.CD_TIPO_ATENDIMENTO,    
     TbRRRelatorioReclamacao.CD_TIPO_RECLAMACAO,    
     TbRRRelatorioReclamacao.DS_MOTIVO,    
	 TbRRRelatorioReclamacao.DS_DESCRICAO,    
     TbRRRelatorioReclamacao.VL_TEMPO_ATENDIMENTO,    
     TbRRRelatorioReclamacao.DS_ARQUIVO_FOTO,    
     TbRRRelatorioReclamacao.DS_TIPO_FOTO,    
     nidUsuarioAtualizacao,    
     dtmDataHoraAtualizacao,    
     TbRRStatus.DS_STATUS_NOME,    
     TbRRStatus.DS_STATUS_NOME_REDUZ,    
     TB_CLIENTE.NM_CLIENTE,    
     TB_TECNICO.NM_TECNICO as TECNICO_SOLICITANTE,    
     TB_PECA.DS_PECA,  
	 TB_PECA.CD_PECA, 
     TbRRRelatorioReclamacao.NM_FORNECEDOR,  
     TbRRRelatorioReclamacao.VL_CUSTO_PECA,  
     --TbRRRelatorioReclamacao.CUSTO_TOTAL,  
     TbRRRelatorioReclamacao.VL_MAO_OBRA  ,
	 TbRRRelatorioReclamacao.CD_GRUPO_RESPONS,
	 TbRRRelatorioReclamacao.ID_OS   ,
     TbRRRelatorioReclamacao.Dt_Criacao 
    FROM dbo.TbRRRelatorioReclamacao (nolock)    
    INNER JOIN  TbRRStatus (nolock) on TbRRStatus.ST_STATUS_RR = TbRRRelatorioReclamacao.ST_STATUS_RR    
    INNER JOIN  TB_CLIENTE (nolock) on TB_CLIENTE.CD_CLIENTE = TbRRRelatorioReclamacao.CD_CLIENTE    
    INNER JOIN  TB_TECNICO (nolock) on TB_TECNICO.CD_TECNICO = TbRRRelatorioReclamacao.CD_TECNICO    
    INNER JOIN  TB_ATIVO_FIXO (nolock) on TB_ATIVO_FIXO.CD_ATIVO_FIXO = TbRRRelatorioReclamacao.CD_ATIVO_FIXO    
    left JOIN  TB_PECA (nolock) on TB_PECA.CD_PECA = TbRRRelatorioReclamacao.CD_PECA    
    
    WHERE     
    ( @p_ID_RELATORIO_RECLAMACAO IS NULL OR ID_RELATORIO_RECLAMACAO = @p_ID_RELATORIO_RECLAMACAO)     
   AND   ( @p_ID_RR_STATUS IS NULL OR TbRRRelatorioReclamacao.ST_STATUS_RR = @p_ID_RR_STATUS)     
   AND   ( @p_CD_TECNICO IS NULL OR TbRRRelatorioReclamacao.CD_TECNICO LIKE @p_CD_TECNICO)     
   AND   ( @p_CD_CLIENTE IS NULL OR TbRRRelatorioReclamacao.CD_CLIENTE LIKE @p_CD_CLIENTE)     
   AND   ( @p_CD_ATIVO_FIXO IS NULL OR TbRRRelatorioReclamacao.CD_ATIVO_FIXO LIKE @p_CD_ATIVO_FIXO)     
   AND   ( @p_CD_PECA IS NULL OR TbRRRelatorioReclamacao.CD_PECA LIKE @p_CD_PECA)     
   AND   ( @p_CD_TIPO_ATENDIMENTO IS NULL OR CD_TIPO_ATENDIMENTO = @p_CD_TIPO_ATENDIMENTO)     
   AND   ( @p_CD_TIPO_RECLAMACAO IS NULL OR CD_TIPO_RECLAMACAO = @p_CD_TIPO_RECLAMACAO) 
   AND   ( @p_ID_OS IS NULL OR TbRRRelatorioReclamacao.ID_OS = @p_ID_OS)
       
  order by Dt_Criacao DESC;
      
  END TRY    
    
 BEGIN CATCH    
    
  SELECT @cdsErrorMessage = ERROR_MESSAGE(),    
    @nidErrorSeverity = ERROR_SEVERITY(),    
    @nidErrorState  = ERROR_STATE();    
    
  -- Use RAISERROR inside the CATCH block to return error    
  -- information about the original error that caused    
  -- execution to jump to the CATCH block.    
  RAISERROR (@cdsErrorMessage, -- Message text.    
       @nidErrorSeverity, -- Severity.    
       @nidErrorState -- State.    
       )    
    
 END CATCH    
    
END




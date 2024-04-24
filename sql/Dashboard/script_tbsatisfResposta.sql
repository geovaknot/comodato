alter table tbSatisfResposta add ID_OS bigint not null default(0)
UPDATE
    tbSatisfResposta
SET
    tbSatisfResposta.ID_OS = RAN.ID_OS
FROM
    tbSatisfResposta SI
INNER JOIN
    tbOS RAN
ON 
    SI.ID_VISITA = RAN.ID_VISITA;
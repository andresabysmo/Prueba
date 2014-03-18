/* Formatted on 2013/08/05 13:26 (Formatter Plus v4.8.6) */
SELECT
  /*+ parallel(f,4)  */
  ---   INDEX(G X_CITEM_DUNNING_DATASRC )  INDEX(G FK_CITEM_CPARTY)*/
  DISTINCT e.NAME ciudad,
  f.cparty_id,
  d.account_id,
  y.citem_id,
  f.fullname,
  NVL (f.personalcode, f.companycode) ci_ruc,
  d.NAME cuenta,
  --y_fd_devuelvefechacontratacion (d.cparty_id, d.account_id ) fechamin,
  (
  SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-02-2013', 'dd-mm-yyyy' ), TO_DATE ('28-02-2013', 'dd-mm-yyyy' ) )
  FROM DUAL
  ) feb13,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-03-2013', 'dd-mm-yyyy' ), TO_DATE ('31-03-2013', 'dd-mm-yyyy' ) )
  FROM DUAL
  ) mar13,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-04-2013', 'dd-mm-yyyy' ), TO_DATE ('30-04-2013', 'dd-mm-yyyy' ) )
  FROM DUAL
  ) abr13,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-05-2013', 'dd-mm-yyyy' ), TO_DATE ('31-05-2013', 'dd-mm-yyyy' ) )
  FROM DUAL
  ) may13,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-06-2013', 'dd-mm-yyyy'), TO_DATE ('30-06-2013', 'dd-mm-yyyy') )
  FROM DUAL
  ) jun13,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-07-2013', 'dd-mm-yyyy' ), TO_DATE ('31-07-2013', 'dd-mm-yyyy' ) )
  FROM DUAL
  ) jul13,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-08-2013', 'dd-mm-yyyy'), TO_DATE ('31-08-2013', 'dd-mm-yyyy') )
  FROM DUAL
  ) ago13,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-09-2013', 'dd-mm-yyyy'), TO_DATE ('30-09-2013', 'dd-mm-yyyy') )
  FROM DUAL
  ) sep13,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-10-2013', 'dd-mm-yyyy' ), TO_DATE ('31-10-2013', 'dd-mm-yyyy' ) )
  FROM DUAL
  ) oct13,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-11-2013', 'dd-mm-yyyy' ), TO_DATE ('30-11-2013', 'dd-mm-yyyy' ) )
  FROM DUAL
  ) nov13,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-12-2013', 'dd-mm-yyyy' ), TO_DATE ('31-12-2013', 'dd-mm-yyyy' ) )
  FROM DUAL
  ) dic13,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-01-2014', 'dd-mm-yyyy' ), TO_DATE ('31-01-2014', 'dd-mm-yyyy' ) )
  FROM DUAL
  ) ene14,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo (d.cparty_id, d.account_id, TO_DATE ('01-02-2014', 'dd-mm-yyyy' ), TO_DATE ('28-02-2014', 'dd-mm-yyyy' ) )
  FROM DUAL
  ) feb14
  /*,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo
  (d.cparty_id,
  d.account_id,
  TO_DATE ('01-06-2013',
  'dd-mm-yyyy'
  ),
  TO_DATE ('30-06-2013',
  'dd-mm-yyyy'
  )
  )
  FROM DUAL) junio13,
  (SELECT y_tvc_production_am.devuelve_estados_nuevo
  (d.cparty_id,
  d.account_id,
  TO_DATE ('01-07-2013',
  'dd-mm-yyyy'
  ),
  TO_DATE ('31-07-2013',
  'dd-mm-yyyy'
  )
  )
  FROM DUAL) julio13*/
FROM tamcpartyaccountd d,
  tamcontractingpartyd f,
  tamcpartynoded e,
  ytv_revistaslog y
WHERE d.cparty_id = f.cparty_id
AND f.cparty_id   > 0
AND e.ID          = d.costcenter_id
AND d.account_id  > 0
AND d.cparty_id   = f.cparty_id
AND d.account_id  = y.account_id
AND d.cparty_id   = y.cparty_id
  --AND d.cparty_id   =4485829

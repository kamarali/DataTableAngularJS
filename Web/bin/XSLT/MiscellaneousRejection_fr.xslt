<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">
  <xsl:output version="1.0" method="xml" encoding="UTF-8" indent="no"/>
  <xsl:param name="LogoFileName"/>
  <xsl:param name="ImagesFolder"/>
  <xsl:template match="/">
    <fo:root>
      <fo:layout-master-set>
        <fo:simple-page-master master-name="first-page-layout" page-height="210mm" page-width="297mm" margin-left="5mm" margin-right="5mm" margin-top="5mm" margin-bottom="5mm">
          <fo:region-before region-name="header">
            <xsl:attribute name="extent">
              <xsl:choose>
                <xsl:when test="count(/InvoiceTemplate/InvoiceHeader/HeaderNotes/Note) > 0">
                  102.5mm
                </xsl:when>
                <xsl:otherwise>
                  95mm
                </xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
          </fo:region-before>
          <fo:region-body region-name="body">
            <xsl:attribute name="margin-top">
              <xsl:choose>
                <xsl:when test="count(/InvoiceTemplate/InvoiceHeader/HeaderNotes/Note) > 0">
                  102.5mm
                </xsl:when>
                <xsl:otherwise>
                  95mm
                </xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
            <xsl:attribute name="margin-bottom">
              <xsl:choose>
                <xsl:when test="count(/InvoiceTemplate/InvoiceFooter/LegalText) > 0">
                  30mm
                </xsl:when>
                <xsl:otherwise>
                  15mm
                </xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
          </fo:region-body>
          <fo:region-after region-name="footer">
            <xsl:attribute name="extent">
              <xsl:choose>
                <xsl:when test="count(/InvoiceTemplate/InvoiceFooter/LegalText) > 0">
                  25mm
                </xsl:when>
                <xsl:otherwise>
                  10mm
                </xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
          </fo:region-after>
        </fo:simple-page-master>
        <fo:simple-page-master master-name="rest-pages-layout" page-height="210mm" page-width="297mm" margin-left="5mm" margin-right="5mm" margin-top="5mm" margin-bottom="5mm">
          <fo:region-before region-name="header-partial" extent="44.6mm"/>
          <fo:region-body region-name="body" margin-top="44.6mm">
            <xsl:attribute name="margin-bottom">
              <xsl:choose>
                <xsl:when test="count(/InvoiceTemplate/InvoiceFooter/LegalText) > 0">
                  30mm
                </xsl:when>
                <xsl:otherwise>
                  15mm
                </xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
          </fo:region-body>
          <fo:region-after region-name="footer">
            <xsl:attribute name="extent">
              <xsl:choose>
                <xsl:when test="count(/InvoiceTemplate/InvoiceFooter/LegalText) > 0">
                  25mm
                </xsl:when>
                <xsl:otherwise>
                  10mm
                </xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
          </fo:region-after>
        </fo:simple-page-master>
        <fo:page-sequence-master master-name="default-page">
          <fo:repeatable-page-master-alternatives>
            <fo:conditional-page-master-reference master-reference="first-page-layout" page-position="first"/>
            <fo:conditional-page-master-reference master-reference="rest-pages-layout" page-position="rest"/>
          </fo:repeatable-page-master-alternatives>
        </fo:page-sequence-master>
      </fo:layout-master-set>
      <fo:page-sequence master-reference="default-page">
        <fo:static-content flow-name="header">
          <xsl:call-template name="header">
            <xsl:with-param name="ShowFullHeader" select="true()"/>
          </xsl:call-template>
        </fo:static-content>
        <fo:static-content flow-name="header-partial">
          <xsl:call-template name="header">
            <xsl:with-param name="ShowFullHeader" select="false()"/>
          </xsl:call-template>
        </fo:static-content>
        <fo:static-content flow-name="footer">
          <xsl:call-template name="show-footer-text"/>
        </fo:static-content>
        <fo:flow flow-name="body">
          <xsl:call-template name="body-content"/>
          <fo:block id="last-page"/>
        </fo:flow>
      </fo:page-sequence>
    </fo:root>
  </xsl:template>
  <xsl:template name="header">
    <xsl:param name="ShowFullHeader" select="true()"/>
    <xsl:choose>
      <xsl:when test="$ShowFullHeader">
        <xsl:call-template name="header-first"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="header-rest"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="header-first">
    <xsl:call-template name="watermarktemplate"/>
    <xsl:if test="count(/InvoiceTemplate/InvoiceHeader/HeaderNotes/Note) > 0">
      <fo:block-container position="absolute" top="19.7mm" width="240mm" height="100%">
        <fo:block position="absolute">
          <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
            <fo:table-column column-width="287mm"/>
            <fo:table-body>
              <fo:table-row height="83mm">
                <fo:table-cell xsl:use-attribute-sets="borderL borderR borderT borderB"/>
              </fo:table-row>
            </fo:table-body>
          </fo:table>
        </fo:block>
      </fo:block-container>
    </xsl:if>
    <xsl:if test="count(/InvoiceTemplate/InvoiceHeader/HeaderNotes/Note) = 0">
      <fo:block-container position="absolute" top="19.7mm" height="100%">
        <fo:block position="absolute">
          <fo:table table-layout="fixed" width="287mm" inline-progression-dimension="auto">
            <fo:table-column column-width="95mm"/>
            <fo:table-column column-width="proportional-column-width(1)"/>
            <fo:table-column column-width="95mm"/>
            <fo:table-body>
              <fo:table-row height="75mm">
                <fo:table-cell xsl:use-attribute-sets="borderL borderT borderB"/>
                <fo:table-cell xsl:use-attribute-sets="headerCenterColumn borderT borderB"/>
                <fo:table-cell xsl:use-attribute-sets="borderR borderT borderB"/>
              </fo:table-row>
            </fo:table-body>
          </fo:table>
        </fo:block>
      </fo:block-container>
    </xsl:if>
    <fo:block>
      <fo:table width="100%" table-layout="fixed">
        <fo:table-column column-width="proportional-column-width(1)"/>
        <fo:table-column column-width="200mm"/>
        <fo:table-column column-width="proportional-column-width(1)"/>
        <fo:table-body>
          <fo:table-row height="20mm">
            <fo:table-cell number-rows-spanned="2">
              <fo:block>
                <fo:external-graphic>
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ImagesFolder, $LogoFileName)"/>
                  </xsl:attribute>
                </fo:external-graphic>
              </fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="pageTitle" number-rows-spanned="2">
              <fo:block>Facture de rejet Miscellaneous</fo:block>
            </fo:table-cell>
            <fo:table-cell text-align="right" xsl:use-attribute-sets="pageHeader">
              <xsl:choose>
                <xsl:when test="/InvoiceTemplate/InvoiceHeader/DigitallySigned = 'Yes'">
                  <fo:block>
                    <xsl:text>Facture dématérialisée&#160;Signée électroniquement</xsl:text>
                  </fo:block>
                </xsl:when>
                <xsl:otherwise>
                  <fo:block height="2mm">&#160;</fo:block>
                </xsl:otherwise>
              </xsl:choose>
            </fo:table-cell>
          </fo:table-row>
        </fo:table-body>
      </fo:table>
    </fo:block>
    <fo:block>
      <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
        <fo:table-column/>
        <fo:table-body>
          <fo:table-row>
            <fo:table-cell>
              <fo:block>
                <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                  <fo:table-column/>
                  <fo:table-body>
                    <fo:table-row>
                      <fo:table-cell>
                        <fo:block>
                          <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                            <fo:table-column column-width="95mm"/>
                            <fo:table-column column-width="proportional-column-width(1)"/>
                            <fo:table-column column-width="95mm"/>
                            <fo:table-body>
                              <fo:table-row>
                                <fo:table-cell>
                                  <xsl:if test="count(/InvoiceTemplate/InvoiceHeader/HeaderNotes/Note) > 0">
                                    <xsl:attribute name="border-bottom">
                                      #000000 0.5pt solid
                                    </xsl:attribute>
                                  </xsl:if>
                                  <fo:block padding-before="1mm" padding-after="4mm">
                                    <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                                      <fo:table-column/>
                                      <fo:table-column column-width="3mm"/>
                                      <fo:table-column/>
                                      <fo:table-body>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Entité facturante /nom</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingEntity/EntityName"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Désignation de l'entité &amp; Code compagnie</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="concat(/InvoiceTemplate/InvoiceHeader/BillingEntity/EntityDesignator, '-', /InvoiceTemplate/InvoiceHeader/BillingEntity/EntityNumericCode)"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Identification de la localisation</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/LocationID"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Contact</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingEntity/ContactPerson"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Rue</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/AddressLine1"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <xsl:if test="/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/AddressLine2  !=''">
                                          <fo:table-row line-height="10pt">
                                            <fo:table-cell column-number="3">
                                              <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                                <xsl:value-of select="concat(/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/AddressLine2, '&#160;')"/>
                                              </fo:block>
                                            </fo:table-cell>
                                          </fo:table-row>
                                        </xsl:if>
                                        <xsl:if test="/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/AddressLine3  !=''">
                                          <fo:table-row line-height="10pt">
                                            <fo:table-cell column-number="3">
                                              <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                                <xsl:value-of select="concat(/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/AddressLine3, '&#160;')"/>
                                              </fo:block>
                                            </fo:table-cell>
                                          </fo:table-row>
                                        </xsl:if>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Ville</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/City"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Région</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/Region"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Code postal</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/PostalCode"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Pays</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/Country"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Numéro de TVA</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingEntity/TaxRegistrationNumber"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Numéro d’enregistrement</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingEntity/CompanyRegistrationNumber"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <xsl:if test="/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/AddressLine2  =''">
                                          <fo:table-row>
                                            <xsl:attribute name="height">
                                              <xsl:choose>
                                                <xsl:when test="/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/AddressLine3 =''">
                                                  10.9mm
                                                </xsl:when>
                                                <xsl:otherwise>
                                                  15mm
                                                </xsl:otherwise>
                                              </xsl:choose>
                                            </xsl:attribute>
                                            <fo:table-cell>
                                              <fo:block>&#160;</fo:block>
                                            </fo:table-cell>
                                          </fo:table-row>
                                        </xsl:if>
                                        <xsl:if test="/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/AddressLine3  =''">
                                          <fo:table-row>
                                            <xsl:attribute name="height">
                                              <xsl:choose>
                                                <xsl:when test="/InvoiceTemplate/InvoiceHeader/BillingEntity/Location/AddressLine2 =''">
                                                  10.9mm
                                                </xsl:when>
                                                <xsl:otherwise>
                                                  15mm
                                                </xsl:otherwise>
                                              </xsl:choose>
                                            </xsl:attribute>
                                            <fo:table-cell>
                                              <fo:block>&#160;</fo:block>
                                            </fo:table-cell>
                                          </fo:table-row>
                                        </xsl:if>
                                      </fo:table-body>
                                    </fo:table>
                                  </fo:block>
                                </fo:table-cell>
                                <fo:table-cell xsl:use-attribute-sets="headerCenterColumn">
                                  <xsl:if test="count(/InvoiceTemplate/InvoiceHeader/HeaderNotes/Note) > 0">
                                    <xsl:attribute name="border-bottom">
                                      #000000 0.5pt solid
                                    </xsl:attribute>
                                  </xsl:if>
                                  <fo:block padding-before="1mm" padding-after="4mm">
                                    <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                                      <fo:table-column/>
                                      <fo:table-column column-width="3mm"/>
                                      <fo:table-column/>
                                      <fo:table-body>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Numéro de facture</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldDataBold">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/InvoiceNumber"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Date</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/InvoiceDate"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Mois / Année</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingMonthAndYear"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Période</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingPeriod"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <!---CMP477: new tax invoice number field added-->
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Numéro de facture fiscale</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/TaxInvoiceNumber"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Catégorie de facturation</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/ChargeCategory"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Monnaie de facturation</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/CurrencyOfBilling"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <xsl:if test="/InvoiceTemplate/InvoiceSummary/ClearanceCurrencyCode !=''">
                                          <fo:table-row line-height="10pt">
                                            <fo:table-cell>
                                              <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Devise de Compensation</fo:block>
                                            </fo:table-cell>
                                            <fo:table-cell column-number="3">
                                              <fo:block xsl:use-attribute-sets="fieldData">
                                                <xsl:value-of select="/InvoiceTemplate/InvoiceSummary/ClearanceCurrencyCode"/>
                                              </fo:block>
                                            </fo:table-cell>
                                          </fo:table-row>
                                        </xsl:if>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Méthode de facturation</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/SettlementMethod"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Numéro de commande</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/PONumber"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Numéro de la facture initiale</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/OriginalInvoiceNumber"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Période de facturation initiale</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/OriginalSettlementMonthPeriod"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Pièces jointes</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/Attachments/AttachmentIndicatorOriginal"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Détails des postes de facturation</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/LineItemDetails"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <xsl:if test="count(/InvoiceTemplate/InvoiceHeader/CHAgreementIndicator) > 0">
                                          <fo:table-row line-height="10pt">
                                            <fo:table-cell>
                                              <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Indicateur d’accord CH</fo:block>
                                            </fo:table-cell>
                                            <fo:table-cell column-number="3">
                                              <fo:block xsl:use-attribute-sets="fieldData">
                                                <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/CHAgreementIndicator"/>
                                              </fo:block>
                                            </fo:table-cell>
                                          </fo:table-row>
                                        </xsl:if>
                                        <xsl:if test="count(/InvoiceTemplate/InvoiceHeader/CHDueDate) > 0">
                                          <fo:table-row line-height="10pt">
                                            <fo:table-cell>
                                              <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Date d’échéance CH</fo:block>
                                            </fo:table-cell>
                                            <fo:table-cell column-number="3">
                                              <fo:block xsl:use-attribute-sets="fieldData">
                                                <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/CHDueDate"/>
                                              </fo:block>
                                            </fo:table-cell>
                                          </fo:table-row>
                                        </xsl:if>
                                      </fo:table-body>
                                    </fo:table>
                                  </fo:block>
                                </fo:table-cell>
                                <fo:table-cell>
                                  <xsl:if test="count(/InvoiceTemplate/InvoiceHeader/HeaderNotes/Note) > 0">
                                    <xsl:attribute name="border-bottom">
                                      #000000 0.5pt solid
                                    </xsl:attribute>
                                  </xsl:if>
                                  <fo:block padding-before="1mm" padding-after="4mm">
                                    <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                                      <fo:table-column/>
                                      <fo:table-column column-width="3mm"/>
                                      <fo:table-column/>
                                      <fo:table-body>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Entitée facturée /nom</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BilledEntity/EntityName"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Désignation de l'entité &amp; Code compagnie</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="concat(/InvoiceTemplate/InvoiceHeader/BilledEntity/EntityDesignator, '-', /InvoiceTemplate/InvoiceHeader/BilledEntity/EntityNumericCode)"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Identification de la localisation</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/LocationID"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Contact</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BilledEntity/ContactPerson"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Rue</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/AddressLine1"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <xsl:if test="/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/AddressLine2  !=''">
                                          <fo:table-row line-height="10pt">
                                            <fo:table-cell column-number="3">
                                              <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                                <xsl:value-of select="concat(/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/AddressLine2, '&#160;')"/>
                                              </fo:block>
                                            </fo:table-cell>
                                          </fo:table-row>
                                        </xsl:if>
                                        <xsl:if test="/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/AddressLine3  !=''">
                                          <fo:table-row line-height="10pt">
                                            <fo:table-cell column-number="3">
                                              <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                                <xsl:value-of select="concat(/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/AddressLine3, '&#160;')"/>
                                              </fo:block>
                                            </fo:table-cell>
                                          </fo:table-row>
                                        </xsl:if>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Ville</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/City"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Région</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/Region"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Code postal</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/PostalCode"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Pays</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/Country"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Numéro de TVA</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BilledEntity/TaxRegistrationNumber"/>
                                              <xsl:text>&#160;</xsl:text>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Numéro d’enregistrement</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData" margin-right="2mm">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BilledEntity/CompanyRegistrationNumber"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <xsl:if test="/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/AddressLine2  =''">
                                          <fo:table-row>
                                            <xsl:attribute name="height">
                                              <xsl:choose>
                                                <xsl:when test="/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/AddressLine3 =''">
                                                  10.9mm
                                                </xsl:when>
                                                <xsl:otherwise>
                                                  15mm
                                                </xsl:otherwise>
                                              </xsl:choose>
                                            </xsl:attribute>
                                            <fo:table-cell>
                                              <fo:block>&#160;</fo:block>
                                            </fo:table-cell>
                                          </fo:table-row>
                                        </xsl:if>
                                        <xsl:if test="/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/AddressLine3  =''">
                                          <fo:table-row>
                                            <xsl:attribute name="height">
                                              <xsl:choose>
                                                <xsl:when test="/InvoiceTemplate/InvoiceHeader/BilledEntity/Location/AddressLine2 =''">
                                                  10.9mm
                                                </xsl:when>
                                                <xsl:otherwise>
                                                  15mm
                                                </xsl:otherwise>
                                              </xsl:choose>
                                            </xsl:attribute>
                                            <fo:table-cell>
                                              <fo:block>&#160;</fo:block>
                                            </fo:table-cell>
                                          </fo:table-row>
                                        </xsl:if>
                                      </fo:table-body>
                                    </fo:table>
                                  </fo:block>
                                </fo:table-cell>
                              </fo:table-row>
                            </fo:table-body>
                          </fo:table>
                        </fo:block>
                      </fo:table-cell>
                    </fo:table-row>
                    <xsl:if test="count(/InvoiceTemplate/InvoiceHeader/HeaderNotes/Note) > 0">
                      <fo:table-row>
                        <fo:table-cell>
                          <fo:block>
                            <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                              <fo:table-column column-width="16mm"/>
                              <fo:table-column/>
                              <fo:table-body>
                                <fo:table-row height="1mm">
                                  <fo:table-cell number-columns-spanned="2"/>
                                </fo:table-row>
                                <fo:table-row height="10mm">
                                  <fo:table-cell>
                                    <fo:block xsl:use-attribute-sets="fieldLabel">&#160;Texte d'en-tête :</fo:block>
                                  </fo:table-cell>
                                  <fo:table-cell>
                                    <fo:block xsl:use-attribute-sets="fieldData">
                                      <xsl:for-each select="/InvoiceTemplate/InvoiceHeader/HeaderNotes/Note">
                                        <xsl:value-of select="concat(., ' ')"/>
                                      </xsl:for-each>
                                    </fo:block>
                                  </fo:table-cell>
                                </fo:table-row>
                              </fo:table-body>
                            </fo:table>
                          </fo:block>
                        </fo:table-cell>
                      </fo:table-row>
                    </xsl:if>
                  </fo:table-body>
                </fo:table>
              </fo:block>
            </fo:table-cell>
          </fo:table-row>
        </fo:table-body>
      </fo:table>
    </fo:block>
  </xsl:template>
  <xsl:template name="header-rest">
    <xsl:call-template name="watermarktemplateRest"/>
    <fo:block-container position="absolute" top="19.7mm" width="285mm">
      <fo:block position="absolute">
        <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
          <fo:table-column column-width="287mm"/>
          <fo:table-body>
            <fo:table-row>
              <fo:table-cell>
                <fo:block>
                  <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                    <fo:table-column column-width="95mm"/>
                    <fo:table-column column-width="proportional-column-width(1)"/>
                    <fo:table-column column-width="95mm"/>
                    <fo:table-body>
                      <fo:table-row height="25mm">
                        <fo:table-cell xsl:use-attribute-sets="borderL borderT borderB"/>
                        <fo:table-cell xsl:use-attribute-sets="headerCenterColumn borderB borderT"/>
                        <fo:table-cell xsl:use-attribute-sets="borderR borderT borderB"/>
                      </fo:table-row>
                    </fo:table-body>
                  </fo:table>
                </fo:block>
              </fo:table-cell>
            </fo:table-row>
          </fo:table-body>
        </fo:table>
      </fo:block>
    </fo:block-container>
    <fo:block>
      <fo:table width="100%" table-layout="fixed">
        <fo:table-column column-width="proportional-column-width(1)"/>
        <fo:table-column column-width="200mm"/>
        <fo:table-column column-width="proportional-column-width(1)"/>
        <fo:table-body>
          <fo:table-row height="20mm">
            <fo:table-cell number-rows-spanned="2">
              <fo:block>
                <fo:external-graphic>
                  <xsl:attribute name="src">
                    <xsl:value-of select="concat($ImagesFolder, $LogoFileName)"/>
                  </xsl:attribute>
                </fo:external-graphic>
              </fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="pageTitle" number-rows-spanned="2">
              <fo:block>Facture de rejet Miscellaneous</fo:block>
            </fo:table-cell>
            <fo:table-cell text-align="right" xsl:use-attribute-sets="pageHeader">
              <xsl:choose>
                <xsl:when test="/InvoiceTemplate/InvoiceHeader/DigitallySigned = 'Yes'">
                  <fo:block>
                    <xsl:text>Facture dématérialisée&#160;Signée électroniquement</xsl:text>
                  </fo:block>
                </xsl:when>
                <xsl:otherwise>
                  <fo:block height="2mm">&#160;</fo:block>
                </xsl:otherwise>
              </xsl:choose>
            </fo:table-cell>
          </fo:table-row>
        </fo:table-body>
      </fo:table>
    </fo:block>
    <fo:block>
      <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
        <fo:table-column column-width="proportional-column-width(1)"/>
        <fo:table-body>
          <fo:table-row>
            <fo:table-cell display-align="center">
              <fo:block>
                <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                  <fo:table-column/>
                  <fo:table-body>
                    <fo:table-row>
                      <fo:table-cell>
                        <fo:block>
                          <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                            <fo:table-column column-width="95mm"/>
                            <fo:table-column column-width="proportional-column-width(1)"/>
                            <fo:table-column column-width="95mm"/>
                            <fo:table-body>
                              <fo:table-row>
                                <fo:table-cell>
                                  <fo:block padding-before="1mm" padding-after="4mm">
                                    <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                                      <fo:table-column/>
                                      <fo:table-column column-width="3mm"/>
                                      <fo:table-column/>
                                      <fo:table-body>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Entité facturante /nom</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BillingEntity/EntityName"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                      </fo:table-body>
                                    </fo:table>
                                  </fo:block>
                                </fo:table-cell>
                                <fo:table-cell xsl:use-attribute-sets="headerCenterColumn">
                                  <fo:block padding-before="1mm">
                                    <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                                      <fo:table-column/>
                                      <fo:table-column column-width="3mm"/>
                                      <fo:table-column/>
                                      <fo:table-body>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Numéro de facture</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldDataBold">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/InvoiceNumber"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Date</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/InvoiceDate"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                      </fo:table-body>
                                    </fo:table>
                                  </fo:block>
                                </fo:table-cell>
                                <fo:table-cell>
                                  <fo:block padding-before="1mm" padding-after="4mm">
                                    <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                                      <fo:table-column/>
                                      <fo:table-column column-width="3mm"/>
                                      <fo:table-column/>
                                      <fo:table-body>
                                        <fo:table-row line-height="10pt">
                                          <fo:table-cell>
                                            <fo:block xsl:use-attribute-sets="fieldLabel" text-align="end">Entitée facturée /nom</fo:block>
                                          </fo:table-cell>
                                          <fo:table-cell column-number="3">
                                            <fo:block xsl:use-attribute-sets="fieldData">
                                              <xsl:value-of select="/InvoiceTemplate/InvoiceHeader/BilledEntity/EntityName"/>
                                            </fo:block>
                                          </fo:table-cell>
                                        </fo:table-row>
                                      </fo:table-body>
                                    </fo:table>
                                  </fo:block>
                                </fo:table-cell>
                              </fo:table-row>
                            </fo:table-body>
                          </fo:table>
                        </fo:block>
                      </fo:table-cell>
                    </fo:table-row>
                  </fo:table-body>
                </fo:table>
              </fo:block>
            </fo:table-cell>
          </fo:table-row>
        </fo:table-body>
      </fo:table>
    </fo:block>
  </xsl:template>
  <xsl:template name="watermarktemplate">
    <fo:block-container position="absolute" top="26mm" height="11.5cm">
      <fo:block position="absolute">
        <fo:table table-layout="fixed" width="287mm">
          <fo:table-column column-width="95mm"/>
          <fo:table-column column-width="proportional-column-width(1)"/>
          <fo:table-column column-width="95mm"/>
          <fo:table-body>
            <fo:table-row>
              <fo:table-cell xsl:use-attribute-sets="pageWaterMark">
                <fo:block>
                  <fo:inline color="#D7E9F8">
                    <xsl:if test="/InvoiceTemplate[not(preceding-sibling::*)]/InvoiceHeader/DigitallySigned = 'XML'">
                      <xsl:text>Copie</xsl:text>
                    </xsl:if>
                  </fo:inline>
                </fo:block>
              </fo:table-cell>
              <fo:table-cell xsl:use-attribute-sets="pageWaterMark" column-number="3">
                <fo:block>
                  <fo:inline color="#D7E9F8">
                    <xsl:if test="/InvoiceTemplate[not(preceding-sibling::*)]/InvoiceHeader/DigitallySigned = 'XML'">
                      <xsl:text>Copie</xsl:text>
                    </xsl:if>
                  </fo:inline>
                </fo:block>
              </fo:table-cell>
            </fo:table-row>
          </fo:table-body>
        </fo:table>
      </fo:block>
    </fo:block-container>
  </xsl:template>
  <xsl:template name="watermarktemplateRest">
    <fo:block-container position="absolute" top="15mm" height="11.5cm">
      <fo:block position="absolute">
        <fo:table table-layout="fixed" width="287mm">
          <fo:table-column column-width="95mm"/>
          <fo:table-column column-width="proportional-column-width(1)"/>
          <fo:table-column column-width="95mm"/>
          <fo:table-body>
            <fo:table-row>
              <fo:table-cell xsl:use-attribute-sets="pageWaterMark">
                <fo:block>
                  <fo:inline color="#D7E9F8">
                    <xsl:if test="/InvoiceTemplate[not(preceding-sibling::*)]/InvoiceHeader/DigitallySigned = 'XML'">
                      <xsl:text>Copie</xsl:text>
                    </xsl:if>
                  </fo:inline>
                </fo:block>
              </fo:table-cell>
              <fo:table-cell xsl:use-attribute-sets="pageWaterMark" column-number="3">
                <fo:block>
                  <fo:inline color="#D7E9F8">
                    <xsl:if test="/InvoiceTemplate[not(preceding-sibling::*)]/InvoiceHeader/DigitallySigned = 'XML'">
                      <xsl:text>Copie</xsl:text>
                    </xsl:if>
                  </fo:inline>
                </fo:block>
              </fo:table-cell>
            </fo:table-row>
          </fo:table-body>
        </fo:table>
      </fo:block>
    </fo:block-container>
  </xsl:template>
  <xsl:template name="body-content">
    <fo:block>
      <fo:table table-layout="fixed" width="100%">
        <fo:table-column column-width="proportional-column-width(1)"/>
        <fo:table-body>
          <fo:table-row>
            <fo:table-cell>
              <xsl:if test="(count(/InvoiceTemplate/InvoiceHeader/HeaderNotes/InvoiceData) > 0)">
                <xsl:call-template name="InvoiceData"/>
              </xsl:if>
              <xsl:if test="(count(/InvoiceTemplate/MiscellaneousRejectionLineItems/MiscellaneousRejectionLineItem) > 0) 
                      or (count(/InvoiceTemplate/MiscellaneousRejectionLineItems/SubTotal) > 0)
                      or (count(InvoiceTemplate/InvoiceSummary/SubTotals) > 0)">
                <xsl:call-template name="LineItems"/>
              </xsl:if>
              <xsl:if test="count(//InvoiceSummary/VatBreakDown/VatItem) != 0">
                <xsl:call-template name="VatBreakDown"/>
              </xsl:if>
              <xsl:if test="count(//InvoiceSummary/TaxBreakDown/TaxItem) != 0">
                <xsl:call-template name="TaxBreakDown"/>
              </xsl:if>
              <xsl:if test="count(//InvoiceSummary/AdditionalAmountBreakdown/AdditionalAmountItem) != 0">
                <xsl:call-template name="AmtBreakDown"/>
              </xsl:if>
              <xsl:call-template name="PaymentTerms"/>
              <xsl:if test="count(/InvoiceTemplate/RejectionReasonLegend/RejectionReason) != 0">
                <xsl:call-template name="RejectionReasonLegend"/>
              </xsl:if>
            </fo:table-cell>
          </fo:table-row>
        </fo:table-body>
      </fo:table>
    </fo:block>
  </xsl:template>
  <xsl:template name="InvoiceData">
    <fo:table table-layout="fixed" width="100%" xsl:use-attribute-sets="borderB">
      <fo:table-column column-width="proportional-column-width(1)"/>
      <fo:table-body>
        <xsl:if test="count(/InvoiceTemplate/InvoiceHeader/HeaderNotes/InvoiceData) > 0">
          <fo:table-row>
            <fo:table-cell xsl:use-attribute-sets="borderR borderL borderB">
              <fo:block>
                <fo:table table-layout="fixed" width="100%" inline-progression-dimension="auto">
                  <fo:table-column column-width="16mm"/>
                  <fo:table-column/>
                  <fo:table-body>
                    <fo:table-row height="1mm">
                      <fo:table-cell number-columns-spanned="2"/>
                    </fo:table-row>
                    <fo:table-row height="10mm">
                      <fo:table-cell>
                        <fo:block xsl:use-attribute-sets="fieldLabel">&#160;Données facturées:</fo:block>
                      </fo:table-cell>
                      <fo:table-cell>
                        <fo:block xsl:use-attribute-sets="fieldData">
                          <xsl:for-each select="/InvoiceTemplate/InvoiceHeader/HeaderNotes/InvoiceData">
                            <xsl:value-of select="concat(., ' ; ')"/>
                          </xsl:for-each>
                        </fo:block>
                      </fo:table-cell>
                    </fo:table-row>
                  </fo:table-body>
                </fo:table>
              </fo:block>
            </fo:table-cell>
          </fo:table-row>
        </xsl:if>
      </fo:table-body>
    </fo:table>
  </xsl:template>
  <xsl:template name="LineItems">
    <fo:table table-layout="fixed" width="100%" xsl:use-attribute-sets="borderB">
      <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
      <fo:table-column column-width="12mm"/>
      <fo:table-column column-width="17mm"/>
      <fo:table-column column-width="10mm"/>
      <fo:table-column column-width="17mm"/>
      <fo:table-column column-width="30mm"/>
      <fo:table-column column-width="15mm"/>
      <fo:table-column column-width="proportional-column-width(1)"/>
      <fo:table-column column-width="17mm"/>
      <fo:table-column column-width="17mm"/>
      <fo:table-column column-width="20mm"/>
      <fo:table-column column-width="20mm"/>
      <fo:table-column column-width="17mm"/>
      <fo:table-column column-width="20mm"/>
      <fo:table-column column-width="20mm"/>
      <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
      <fo:table-column column-width="20mm"/>
      <xsl:if test="(count(//MiscellaneousRejectionLineItems/MiscellaneousRejectionLineItem) > 0) 
                      or (count(/InvoiceTemplate/MiscellaneousRejectionLineItems/SubTotal) > 0)">
        <fo:table-header>
          <fo:table-row height="5.8mm" background-color="#3E90CD">
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter borderL">
              <fo:block>Numéro de poste</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter">
              <fo:block>Date de prestation</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter">
              <fo:block>Numéro de poste initial</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter">
              <fo:block>Nature de la prestation</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter">
              <fo:block>Identification du produit</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter">
              <fo:block>Raison du Code Rejet</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter">
              <fo:block>Description</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignRight" padding-right="2mm">
              <fo:block>Quantité</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter">
              <fo:block>Unité de mesure</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignRight" padding-right="2mm">
              <fo:block>Prix unitaire</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignRight" padding-right="2mm">
              <fo:block>Montant de base</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter">
              <fo:block>Montant additionnel</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignRight" padding-right="2mm">
              <fo:block>Taxe</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignRight" padding-right="2mm">
              <fo:block>TVA</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignRight borderR" padding-right="2mm">
              <fo:block>Montant par poste</fo:block>
            </fo:table-cell>
          </fo:table-row>
        </fo:table-header>
      </xsl:if>
      <fo:table-body>
        <xsl:choose>
          <xsl:when test="//MiscellaneousRejectionLineItems/MiscellaneousRejectionLineItem">
            <xsl:for-each select="/InvoiceTemplate/MiscellaneousRejectionLineItems/MiscellaneousRejectionLineItem">
              <xsl:variable name="DateOfServiceBegin" select="DateOfServiceBegin"/>
              <xsl:variable name="DateOfServiceEnd" select="DateOfServiceEnd"/>
              <fo:table-row>
                <xsl:attribute name="background-color">
                  <xsl:choose>
                    <xsl:when test="position() mod 2 = 0">#D7E9F8</xsl:when>
                    <xsl:otherwise>#FFFFFF</xsl:otherwise>
                  </xsl:choose>
                </xsl:attribute>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter paddingTB borderL">
                  <fo:block>
                    <xsl:value-of select="LineNumber"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter paddingTB">
                  <fo:block>
                    <xsl:choose>
                      <xsl:when test="(($DateOfServiceBegin !='') and ($DateOfServiceEnd !=''))">
                        <xsl:value-of select="concat($DateOfServiceBegin, ' to ', $DateOfServiceEnd)"/>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:choose>
                          <xsl:when test="($DateOfServiceBegin != '')">
                            <xsl:value-of select="$DateOfServiceBegin"/>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="$DateOfServiceEnd"/>
                          </xsl:otherwise>
                        </xsl:choose>
                      </xsl:otherwise>
                    </xsl:choose>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter paddingTB">
                  <fo:block>
                    <xsl:value-of select="OriginalLineItemNumber"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter paddingTB">
                  <fo:block>
                    <xsl:value-of select="ChargeCode"/>
                  </fo:block>
                  <xsl:if test="(ChargeCodeType !='')">
                    <fo:block>
                      <xsl:value-of select="concat(' - ', ChargeCodeType)"/>
                    </fo:block>
                  </xsl:if>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter paddingTB">
                  <fo:block>
                    <xsl:value-of select="ProductID"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter paddingTB">
                  <fo:block>
                    <xsl:value-of select="RejectionReasonCode"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter paddingTB">
                  <fo:block>
                    <xsl:value-of select="Description"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight paddingTB" padding-right="2mm">
                  <fo:block>
                    <xsl:value-of select="Quantity"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter paddingTB">
                  <fo:block>
                    <xsl:value-of select="UnitOfMeasure"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight paddingTB" padding-right="2mm">
                  <fo:block>
                    <xsl:value-of select="UnitPrice"/>
                    <xsl:if test="UnitPrice/@SF > '1'">
                      <xsl:text> @ SF </xsl:text>
                      <xsl:value-of select="UnitPrice/@SF"/>
                    </xsl:if>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight paddingTB" padding-right="2mm">
                  <fo:block>
                    <xsl:value-of select="BaseAmount"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight paddingTB" padding-right="2mm">
                  <fo:block>
                    <xsl:value-of select="AdditionalAmount"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight paddingTB" padding-right="2mm">
                  <fo:block>
                    <xsl:value-of select="Tax"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight paddingTB" padding-right="2mm">
                  <fo:block>
                    <xsl:value-of select="Vat"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight paddingTB borderR" padding-right="2mm">
                  <fo:block>
                    <xsl:value-of select="LineAmount"/>
                  </fo:block>
                </fo:table-cell>
              </fo:table-row>
            </xsl:for-each>
          </xsl:when>
          <xsl:otherwise>
            <fo:table-row height="4mm">
              <fo:table-cell number-columns-spanned="13" xsl:use-attribute-sets="borderLR"/>
            </fo:table-row>
          </xsl:otherwise>
        </xsl:choose>
        <xsl:if test="(count(/InvoiceTemplate/MiscellaneousRejectionLineItems/SubTotal) > 0)">
          <fo:table-row>
            <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
            <fo:table-cell number-columns-spanned="8" xsl:use-attribute-sets="borderT borderL">
              <fo:block/>
            </fo:table-cell>
            <fo:table-cell number-columns-spanned="2" xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderT">
              <fo:block padding-before="5mm" wrap-option="no-wrap">Sous total de poste&#160;</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderT" padding-right="2mm">
              <fo:block padding-before="5mm">
                <xsl:value-of select="/InvoiceTemplate/MiscellaneousRejectionLineItems/SubTotal/BaseAmount"/>
              </fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderT" padding-right="2mm">
              <fo:block padding-before="5mm">
                <xsl:value-of select="/InvoiceTemplate/MiscellaneousRejectionLineItems/SubTotal/AdditionalAmount"/>
              </fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderT" padding-right="2mm">
              <fo:block padding-before="5mm">
                <xsl:value-of select="/InvoiceTemplate/MiscellaneousRejectionLineItems/SubTotal/Tax"/>
              </fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderT" padding-right="2mm">
              <fo:block padding-before="5mm">
                <xsl:value-of select="/InvoiceTemplate/MiscellaneousRejectionLineItems/SubTotal/Vat"/>
              </fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderT borderR" padding-right="2mm">
              <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF
							<fo:block padding-before="5mm">
										<xsl:value-of select="/InvoiceTemplate/MiscellaneousRejectionLineItems/SubTotal/LineAmount"/>
							</fo:block>
							-->
              <fo:block/>
            </fo:table-cell>
          </fo:table-row>
        </xsl:if>
        <xsl:if test="count(/InvoiceTemplate/MiscellaneousRejectionLineItems/HeaderSubTotal) > 0">
          <fo:table-row>
            <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
            <fo:table-cell number-columns-spanned="8" xsl:use-attribute-sets="borderL">
              <fo:block/>
            </fo:table-cell>
            <fo:table-cell number-columns-spanned="2" xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight">
              <fo:block padding-before="1mm" wrap-option="no-wrap">Sous total en-tête facture&#160;</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight" padding-right="2mm">
              <fo:block/>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight" padding-right="2mm">
              <fo:block padding-before="1mm">
                <xsl:value-of select="/InvoiceTemplate/MiscellaneousRejectionLineItems/HeaderSubTotal/AdditionalAmount"/>
              </fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight" padding-right="2mm">
              <fo:block padding-before="1mm">
                <xsl:value-of select="/InvoiceTemplate/MiscellaneousRejectionLineItems/HeaderSubTotal/Tax"/>
              </fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight" padding-right="2mm">
              <fo:block padding-before="1mm">
                <xsl:value-of select="/InvoiceTemplate/MiscellaneousRejectionLineItems/HeaderSubTotal/Vat"/>
              </fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderR" padding-right="2mm">
              <fo:block padding-before="1mm"/>
            </fo:table-cell>
          </fo:table-row>
        </xsl:if>
        <xsl:if test="(count(InvoiceTemplate/InvoiceSummary/SubTotals) > 0)">
          <fo:table-row keep-with-next="always" line-height="10pt">
            <fo:table-cell xsl:use-attribute-sets="borderL"/>
            <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight" column-number="9" number-columns-spanned="2">
              <fo:block wrap-option="no-wrap">Montant total de base facturé&#160;</fo:block>
            </fo:table-cell>
            <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderR" column-number="15" padding-right="2mm">
              <fo:block>
                <xsl:value-of select="/InvoiceTemplate/InvoiceSummary/SubTotals/TotalInvoiceBaseAmount"/>
              </fo:block>
            </fo:table-cell>
          </fo:table-row>
          <fo:table-row keep-with-next="always" line-height="10pt">
            <fo:table-cell xsl:use-attribute-sets="borderL"/>
            <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight" column-number="9" number-columns-spanned="2">
              <fo:block wrap-option="no-wrap">Montant total additionnel facturé&#160;</fo:block>
            </fo:table-cell>
            <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderR" column-number="15" padding-right="2mm">
              <fo:block>
                <xsl:value-of select="/InvoiceTemplate/InvoiceSummary/SubTotals/TotalInvoiceAdditionalAmount"/>
              </fo:block>
            </fo:table-cell>
          </fo:table-row>
          <fo:table-row keep-with-next="always" line-height="10pt">
            <fo:table-cell xsl:use-attribute-sets="borderL"/>
            <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight" column-number="9" number-columns-spanned="2">
              <fo:block wrap-option="no-wrap">Montant total taxes facturées&#160;</fo:block>
            </fo:table-cell>
            <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderR" column-number="15" padding-right="2mm">
              <fo:block>
                <xsl:value-of select="/InvoiceTemplate/InvoiceSummary/SubTotals/TotalInvoiceTaxAmount"/>
              </fo:block>
            </fo:table-cell>
          </fo:table-row>
          <fo:table-row keep-with-next="always" line-height="10pt">
            <fo:table-cell xsl:use-attribute-sets="borderL"/>
            <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight" column-number="9" number-columns-spanned="2">
              <fo:block wrap-option="no-wrap">Montant total TVA facturée&#160;</fo:block>
            </fo:table-cell>
            <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderR" column-number="15" padding-right="2mm">
              <fo:block>
                <xsl:value-of select="/InvoiceTemplate/InvoiceSummary/SubTotals/TotalInvoiceVATAmount"/>
              </fo:block>
            </fo:table-cell>
          </fo:table-row>
          <fo:table-row keep-with-next="always" line-height="10pt">
            <fo:table-cell xsl:use-attribute-sets="borderL"/>
            <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
            <fo:table-cell column-number="9" number-columns-spanned="7" xsl:use-attribute-sets="doubleLineDivider borderR">
              <fo:block/>
            </fo:table-cell>
          </fo:table-row>
          <fo:table-row line-height="10pt">
            <fo:table-cell xsl:use-attribute-sets="borderL"/>
            <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
            <fo:table-cell xsl:use-attribute-sets="fieldLabelBold alignVCenter alignRight" column-number="9" number-columns-spanned="2">
              <fo:block wrap-option="no-wrap">Montant du en devise de facturation&#160;</fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignCenter" number-columns-spanned="4">
              <fo:block>
                <xsl:value-of select="/InvoiceTemplate/InvoiceSummary/SubTotals/TotalDueInCurrencyOfBilling/InvoiceCurrencyCode"/>
              </fo:block>
            </fo:table-cell>
            <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderR" padding-right="2mm">
              <fo:block>
                <xsl:value-of select="/InvoiceTemplate/InvoiceSummary/SubTotals/TotalDueInCurrencyOfBilling/Amount"/>
              </fo:block>
            </fo:table-cell>
          </fo:table-row>
          <xsl:if test="/InvoiceTemplate/InvoiceSummary/TotalAmountDueInCurrencyOfClearance !=''">
            <fo:table-row keep-with-next="always"  line-height="10pt">
              <fo:table-cell xsl:use-attribute-sets="borderL"/>
              <!-- SCP457461: SRM - "Line Amount" column in Rejection Invoice PDF -->
              <fo:table-cell xsl:use-attribute-sets="fieldLabelBold alignVCenter alignRight" column-number="9" number-columns-spanned="2">
                <fo:block wrap-option="no-wrap" padding-after="5mm">
                  Total Payable en Devise de Compensation @ Taux de Change&#160;
                  <xsl:value-of select="/InvoiceTemplate/InvoiceSummary/ExchangeRate"/>
                </fo:block>
              </fo:table-cell>
              <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignCenter" number-columns-spanned="4">
                <fo:block padding-after="5mm">
                  <xsl:value-of select="/InvoiceTemplate/InvoiceSummary/ClearanceCurrencyCode"/>
                </fo:block>
              </fo:table-cell>
              <fo:table-cell xsl:use-attribute-sets="fieldDataBold alignVCenter alignRight borderR" padding-right="2mm">
                <fo:block padding-after="5mm">
                  <xsl:value-of select="/InvoiceTemplate/InvoiceSummary/TotalAmountDueInCurrencyOfClearance"/>
                </fo:block>
              </fo:table-cell>
            </fo:table-row>
          </xsl:if>
        </xsl:if>
      </fo:table-body>
    </fo:table>
  </xsl:template>
  <xsl:template name="VatBreakDown">
    <fo:table table-layout="fixed" width="100%" xsl:use-attribute-sets="borderB">
      <fo:table-column column-width="75mm"/>
      <fo:table-column column-width="35mm"/>
      <fo:table-column column-width="76mm"/>
      <fo:table-column column-width="35mm"/>
      <fo:table-column column-width="35mm"/>
      <fo:table-column column-width="31mm"/>
      <fo:table-header>
        <fo:table-row height="5.8mm" background-color="#3E90CD">
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter borderL" padding-left="10mm">
            <fo:block>Détails de la TVA</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter">
            <fo:block>Intitulé</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter">
            <fo:block>Description de TVA</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter">
            <fo:block>Montant de base</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter">
            <fo:block>Taux de TVA</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter borderR">
            <fo:block>Montant de TVA</fo:block>
          </fo:table-cell>
        </fo:table-row>
      </fo:table-header>
      <fo:table-body>
        <xsl:choose>
          <xsl:when test="//InvoiceSummary/VatBreakDown/VatItem">
            <xsl:for-each select="/InvoiceTemplate/InvoiceSummary/VatBreakDown/VatItem">
              <fo:table-row height="4mm">
                <fo:table-cell xsl:use-attribute-sets="borderL"/>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter">
                  <fo:block>
                    <xsl:value-of select="VatLabel"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter">
                  <fo:block>
                    <xsl:value-of select="VatText"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight" padding-right="13mm">
                  <fo:block>
                    <xsl:if test="BaseAmount != '0.000'">
                      <xsl:value-of select="BaseAmount"/>
                    </xsl:if>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight" padding-right="15mm">
                  <fo:block>
                    <xsl:if test="VatRate !=''">
                      <xsl:value-of select="concat(VatRate, '%')"/>
                    </xsl:if>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight borderR" padding-right="12mm">
                  <fo:block>
                    <xsl:if test="VatAmount != '0.000'">
                      <xsl:value-of select="VatAmount"/>
                    </xsl:if>
                  </fo:block>
                </fo:table-cell>
              </fo:table-row>
            </xsl:for-each>
          </xsl:when>
          <xsl:otherwise>
            <fo:table-row height="4mm">
              <fo:table-cell number-columns-spanned="6" xsl:use-attribute-sets="borderLR"/>
            </fo:table-row>
          </xsl:otherwise>
        </xsl:choose>
      </fo:table-body>
    </fo:table>
  </xsl:template>
  <xsl:template name="TaxBreakDown">
    <fo:table table-layout="fixed" width="100%" xsl:use-attribute-sets="borderB">
      <fo:table-column column-width="40mm"/>
      <fo:table-column column-width="35mm"/>
      <fo:table-column column-width="35mm"/>
      <fo:table-column column-width="76mm"/>
      <fo:table-column column-width="35mm"/>
      <fo:table-column column-width="35mm"/>
      <fo:table-column column-width="31mm"/>
      <fo:table-header>
        <fo:table-row height="5.8mm" background-color="#3E90CD">
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter borderL" padding-left="10mm">
            <fo:block>Détails des taxes</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter">
            <fo:block>Niveau</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter">
            <fo:block>Intitulé</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter">
            <fo:block>Description des taxes</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter">
            <fo:block>Montant de base</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter">
            <fo:block>Pourcentage des taxes</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter borderR">
            <fo:block>Montant des taxes</fo:block>
          </fo:table-cell>
        </fo:table-row>
      </fo:table-header>
      <fo:table-body>
        <xsl:choose>
          <xsl:when test="//InvoiceSummary/TaxBreakDown/TaxItem">
            <xsl:for-each select="/InvoiceTemplate/InvoiceSummary/TaxBreakDown/TaxItem">
              <fo:table-row height="4mm">
                <fo:table-cell xsl:use-attribute-sets="borderL"/>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter">
                  <fo:block>
                    <xsl:value-of select="TaxLevel"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter">
                  <fo:block>
                    <xsl:value-of select="TaxLabel"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter">
                  <fo:block>
                    <xsl:value-of select="TaxText"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight" padding-right="13mm">
                  <fo:block>
                    <xsl:if test="BaseAmount != '0.000'">
                      <xsl:value-of select="BaseAmount"/>
                    </xsl:if>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight" padding-right="15mm">
                  <fo:block>
                    <xsl:value-of select="TaxRate"/>
                    <xsl:if test="string(number(TaxRate)) != 'NaN' and TaxRate !=''">
                      <xsl:text>%</xsl:text>
                    </xsl:if>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight borderR" padding-right="12mm">
                  <fo:block>
                    <xsl:value-of select="TaxAmount"/>
                  </fo:block>
                </fo:table-cell>
              </fo:table-row>
            </xsl:for-each>
          </xsl:when>
          <xsl:otherwise>
            <fo:table-row height="4mm">
              <fo:table-cell number-columns-spanned="7" xsl:use-attribute-sets="borderLR"/>
            </fo:table-row>
          </xsl:otherwise>
        </xsl:choose>
      </fo:table-body>
    </fo:table>
  </xsl:template>
  <xsl:template name="AmtBreakDown">
    <fo:table table-layout="fixed" width="100%" xsl:use-attribute-sets="borderB">
      <fo:table-column column-width="75mm"/>
      <fo:table-column column-width="35mm"/>
      <fo:table-column column-width="76mm"/>
      <fo:table-column column-width="35mm"/>
      <fo:table-column column-width="35mm"/>
      <fo:table-column column-width="31mm"/>
      <fo:table-header>
        <fo:table-row height="5.8mm" background-color="#3E90CD">
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter borderL" padding-left="10mm">
            <fo:block>Détails du montant supplémentaire</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter">
            <fo:block>Niveau</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter">
            <fo:block>Type de montant supplémentaire</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter">
            <fo:block>Montant facturable</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter">
            <fo:block>Pourcentage de facturation</fo:block>
          </fo:table-cell>
          <fo:table-cell xsl:use-attribute-sets="tableColumnLabel alignVCenter alignCenter borderR">
            <fo:block>Montant additionnel</fo:block>
          </fo:table-cell>
        </fo:table-row>
      </fo:table-header>
      <fo:table-body>
        <xsl:choose>
          <xsl:when test="//InvoiceSummary/AdditionalAmountBreakdown/AdditionalAmountItem">
            <xsl:for-each select="/InvoiceTemplate/InvoiceSummary/AdditionalAmountBreakdown/AdditionalAmountItem">
              <fo:table-row height="4mm">
                <fo:table-cell xsl:use-attribute-sets="borderL"/>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter">
                  <fo:block>
                    <xsl:value-of select="Level"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter">
                  <fo:block>
                    <xsl:value-of select="AdditionalAmountName"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight" padding-right="13mm">
                  <fo:block>
                    <xsl:value-of select="ChargeableAmount"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight" padding-right="15mm">
                  <fo:block>
                    <xsl:value-of select="ChargeRate"/>
                    <xsl:if test="string(number(ChargeRate)) != 'NaN' and ChargeRate != ''">
                      <xsl:text>%</xsl:text>
                    </xsl:if>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignRight borderR" padding-right="12mm">
                  <fo:block>
                    <xsl:value-of select="AdditionalAmount"/>
                  </fo:block>
                </fo:table-cell>
              </fo:table-row>
            </xsl:for-each>
          </xsl:when>
          <xsl:otherwise>
            <fo:table-row height="4mm">
              <fo:table-cell number-columns-spanned="6" xsl:use-attribute-sets="borderLR"/>
            </fo:table-row>
          </xsl:otherwise>
        </xsl:choose>
      </fo:table-body>
    </fo:table>
  </xsl:template>
  <xsl:template name="PaymentTerms">
    <fo:table table-layout="fixed" width="100%">
      <fo:table-column column-width="proportional-column-width(1)"/>
      <fo:table-header>
        <fo:table-row height="5.8mm" background-color="#3E90CD">
          <fo:table-cell padding-left="10mm" xsl:use-attribute-sets="tableColumnLabel alignVCenter borderLR">
            <fo:block>Conditions de paiement</fo:block>
          </fo:table-cell>
        </fo:table-row>
      </fo:table-header>
      <xsl:variable name="smallcase" select="'abcdefghijklmnopqrstuvwxyz'"/>
      <xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
      <fo:table-body>
        <xsl:choose>
          <xsl:when test="translate(/InvoiceTemplate/InvoiceHeader/SettlementMethod,$smallcase, $uppercase) != 'BILATERAL'">
            <fo:table-row line-height="10pt">
              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter borderBLR" padding-left="10mm">
                <fo:block>PAIEMENT VIA ICH Clearing House</fo:block>
              </fo:table-cell>
            </fo:table-row>
          </xsl:when>
          <xsl:otherwise>
            <fo:table-row keep-together="always">
              <fo:table-cell>
                <fo:block>
                  <fo:table table-layout="fixed" width="100%">
                    <fo:table-column column-width="proportional-column-width(1)"/>
                    <fo:table-column column-width="25mm"/>
                    <fo:table-column column-width="50mm"/>
                    <fo:table-column column-width="45mm"/>
                    <fo:table-column column-width="25mm"/>
                    <fo:table-column column-width="35mm"/>
                    <fo:table-column column-width="35mm"/>
                    <fo:table-header>
                      <fo:table-row height="5.8mm" background-color="lightgray">
                        <fo:table-cell xsl:use-attribute-sets="subTableColumnLabel alignVCenter borderL" padding-left="10mm">
                          <fo:block>Description</fo:block>
                        </fo:table-cell>
                        <fo:table-cell xsl:use-attribute-sets="subTableColumnLabel alignVCenter alignCenter">
                          <fo:block>Date d’échéance</fo:block>
                        </fo:table-cell>
                        <fo:table-cell xsl:use-attribute-sets="subTableColumnLabel alignVCenter alignCenter">
                          <fo:block>Termes</fo:block>
                        </fo:table-cell>
                        <fo:table-cell xsl:use-attribute-sets="subTableColumnLabel alignVCenter alignCenter">
                          <fo:block>Information bancaire</fo:block>
                        </fo:table-cell>
                        <fo:table-cell xsl:use-attribute-sets="subTableColumnLabel alignVCenter alignCenter">
                          <fo:block>Contact</fo:block>
                        </fo:table-cell>
                        <fo:table-cell xsl:use-attribute-sets="subTableColumnLabel alignVCenter alignCenter">
                          <fo:block>Code de la banque</fo:block>
                        </fo:table-cell>
                        <fo:table-cell xsl:use-attribute-sets="subTableColumnLabel alignVCenter alignCenter borderR">
                          <fo:block>Code branche</fo:block>
                        </fo:table-cell>
                      </fo:table-row>
                    </fo:table-header>
                    <fo:table-body>
                      <xsl:choose>
                        <xsl:when test="/InvoiceTemplate/InvoiceSummary/PaymentTerms">
                          <xsl:for-each select="/InvoiceTemplate/InvoiceSummary/PaymentTerms">
                            <fo:table-row height="4mm">
                              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter borderL" padding-left="10mm">
                                <fo:block>
                                  <xsl:value-of select="Description"/>
                                </fo:block>
                              </fo:table-cell>
                              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter">
                                <fo:block>
                                  <xsl:value-of select="DueDate"/>
                                </fo:block>
                              </fo:table-cell>
                              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter">
                                <fo:block>
                                  <xsl:value-of select="Terms"/>
                                </fo:block>
                              </fo:table-cell>
                              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter">
                                <fo:block>
                                  <xsl:value-of select="BankName"/>
                                </fo:block>
                              </fo:table-cell>
                              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter">
                                <fo:block>
                                  <xsl:value-of select="ContactPerson"/>
                                </fo:block>
                              </fo:table-cell>
                              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter">
                                <fo:block>
                                  <xsl:value-of select="BankCode"/>
                                </fo:block>
                              </fo:table-cell>
                              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter borderR">
                                <fo:block>
                                  <xsl:value-of select="BranchCode"/>
                                </fo:block>
                              </fo:table-cell>
                            </fo:table-row>
                          </xsl:for-each>
                        </xsl:when>
                        <xsl:otherwise>
                          <fo:table-row height="4mm">
                            <fo:table-cell number-columns-spanned="6" xsl:use-attribute-sets="borderLR"/>
                          </fo:table-row>
                        </xsl:otherwise>
                      </xsl:choose>
                    </fo:table-body>
                  </fo:table>
                </fo:block>
              </fo:table-cell>
            </fo:table-row>
            <fo:table-row keep-together="always">
              <fo:table-cell>
                <fo:block>
                  <fo:table table-layout="fixed" width="100%" xsl:use-attribute-sets="borderB">
                    <fo:table-column column-width="proportional-column-width(1)"/>
                    <fo:table-column column-width="proportional-column-width(1)"/>
                    <fo:table-column column-width="proportional-column-width(1)"/>
                    <fo:table-column column-width="proportional-column-width(1)"/>
                    <fo:table-column column-width="proportional-column-width(1)"/>
                    <fo:table-header>
                      <fo:table-row height="5.8mm" background-color="lightgray">
                        <fo:table-cell xsl:use-attribute-sets="subTableColumnLabel alignVCenter alignCenter borderL" padding-left="10mm">
                          <fo:block>IBAN</fo:block>
                        </fo:table-cell>
                        <fo:table-cell xsl:use-attribute-sets="subTableColumnLabel alignVCenter alignCenter">
                          <fo:block>SWIFT</fo:block>
                        </fo:table-cell>
                        <fo:table-cell xsl:use-attribute-sets="subTableColumnLabel alignVCenter alignCenter">
                          <fo:block>Numéro du compte bancaire</fo:block>
                        </fo:table-cell>
                        <fo:table-cell xsl:use-attribute-sets="subTableColumnLabel alignVCenter alignCenter">
                          <fo:block>Code devise du compte bancaire</fo:block>
                        </fo:table-cell>
                        <fo:table-cell xsl:use-attribute-sets="subTableColumnLabel alignVCenter alignCenter borderR">
                          <fo:block>Numéro de référence de la facture</fo:block>
                        </fo:table-cell>
                      </fo:table-row>
                    </fo:table-header>
                    <fo:table-body>
                      <xsl:choose>
                        <xsl:when test="//InvoiceSummary/PaymentTerms">
                          <xsl:for-each select="/InvoiceTemplate/InvoiceSummary/PaymentTerms">
                            <fo:table-row height="4mm">
                              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter borderL" padding-left="10mm">
                                <fo:block>
                                  <xsl:value-of select="IBAN"/>
                                </fo:block>
                              </fo:table-cell>
                              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter">
                                <fo:block>
                                  <xsl:value-of select="SWIFT"/>
                                </fo:block>
                              </fo:table-cell>
                              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter">
                                <fo:block>
                                  <xsl:value-of select="BankAccountNumber"/>
                                </fo:block>
                              </fo:table-cell>
                              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter">
                                <fo:block>
                                  <xsl:value-of select="BankCurrencyCode"/>
                                </fo:block>
                              </fo:table-cell>
                              <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter borderR">
                                <fo:block>
                                  <xsl:value-of select="ReferenceInvoiceNumber"/>
                                </fo:block>
                              </fo:table-cell>
                            </fo:table-row>
                          </xsl:for-each>
                        </xsl:when>
                        <xsl:otherwise>
                          <fo:table-row height="4mm">
                            <fo:table-cell number-columns-spanned="5" xsl:use-attribute-sets="borderLR"/>
                          </fo:table-row>
                        </xsl:otherwise>
                      </xsl:choose>
                    </fo:table-body>
                  </fo:table>
                </fo:block>
              </fo:table-cell>
            </fo:table-row>
          </xsl:otherwise>
        </xsl:choose>
      </fo:table-body>
    </fo:table>
  </xsl:template>
  <xsl:template name="RejectionReasonLegend">
    <fo:table table-layout="fixed" width="100%" xsl:use-attribute-sets="borderB">
      <fo:table-column column-width="12mm"/>
      <fo:table-column column-width="proportional-column-width(1)"/>
      <fo:table-header>

        <fo:table-row height="5.8mm" background-color="#3E90CD">
          <fo:table-cell  xsl:use-attribute-sets="tableColumnLabel alignVCenter borderLR" number-columns-spanned="2" padding-left="10mm">
            <fo:block>Légende de la Raison du Code Rejet indiquée dans les Postes de Facturation</fo:block>
          </fo:table-cell>
        </fo:table-row>
      </fo:table-header>
      <fo:table-body>
        <xsl:choose>
          <xsl:when test="(count(//RejectionReasonLegend/RejectionReason) > 0)">
            <xsl:for-each select="/InvoiceTemplate/RejectionReasonLegend/RejectionReason">
              <fo:table-row line-height="10pt">
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter alignCenter paddingTB borderL">
                  <fo:block>
                    <xsl:value-of select="Code"/>
                  </fo:block>
                </fo:table-cell>
                <fo:table-cell xsl:use-attribute-sets="fieldData alignVCenter paddingTB borderR">
                  <fo:block>
                    <xsl:value-of select="Description"/>
                  </fo:block>
                </fo:table-cell>
              </fo:table-row>
            </xsl:for-each>
          </xsl:when>
          <xsl:otherwise>
            <fo:table-row height="4mm">
              <fo:table-cell number-columns-spanned="2" xsl:use-attribute-sets="borderL"/>
            </fo:table-row>
          </xsl:otherwise>
        </xsl:choose>
      </fo:table-body>
    </fo:table>
  </xsl:template>
  <xsl:template name="show-footer-text">
    <fo:block padding-after="1mm" xsl:use-attribute-sets="fieldDataSmall">
      <xsl:for-each select="/InvoiceTemplate/InvoiceFooter/LegalText">
        <xsl:value-of select="concat(., ' ')"/>
      </xsl:for-each>
    </fo:block>
    <fo:block xsl:use-attribute-sets="fieldData">
      <xsl:text>page&#160;</xsl:text>
      <fo:page-number/>
      <xsl:text>&#160;de&#160;</xsl:text>
      <fo:page-number-citation ref-id="last-page"/>
    </fo:block>
  </xsl:template>
  <xsl:attribute-set name="pageTitle">
    <xsl:attribute name="font-family">Calibri</xsl:attribute>
    <xsl:attribute name="font-size">16pt</xsl:attribute>
    <xsl:attribute name="font-weight">bold</xsl:attribute>
    <xsl:attribute name="color">#103F74</xsl:attribute>
    <xsl:attribute name="text-align">center</xsl:attribute>
    <xsl:attribute name="display-align">before</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="pageHeader">
    <xsl:attribute name="font-family">Calibri</xsl:attribute>
    <xsl:attribute name="font-size">10pt</xsl:attribute>
    <xsl:attribute name="font-weight">bold</xsl:attribute>
    <xsl:attribute name="color">#103F74</xsl:attribute>
    <xsl:attribute name="text-align">right</xsl:attribute>
    <xsl:attribute name="display-align">before</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="alignCenter">
    <xsl:attribute name="text-align">center</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="alignVCenter">
    <xsl:attribute name="display-align">center</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="alignRight">
    <xsl:attribute name="text-align">right</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="fieldLabel">
    <xsl:attribute name="font-family">Arial</xsl:attribute>
    <xsl:attribute name="font-size">6pt</xsl:attribute>
    <xsl:attribute name="color">#103F74</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="fieldLabelBold">
    <xsl:attribute name="font-family">Arial</xsl:attribute>
    <xsl:attribute name="font-size">6pt</xsl:attribute>
    <xsl:attribute name="color">#103F74</xsl:attribute>
    <xsl:attribute name="font-weight">bold</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="tableColumnLabel">
    <xsl:attribute name="font-family">Arial</xsl:attribute>
    <xsl:attribute name="font-size">6pt</xsl:attribute>
    <xsl:attribute name="font-weight">bold</xsl:attribute>
    <xsl:attribute name="color">#FFFFFF</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="subTableColumnLabel">
    <xsl:attribute name="font-family">Arial</xsl:attribute>
    <xsl:attribute name="font-size">6pt</xsl:attribute>
    <xsl:attribute name="color">#3E90CD</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="fieldData">
    <xsl:attribute name="font-family">Arial</xsl:attribute>
    <xsl:attribute name="font-size">6pt</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="fieldDataBold">
    <xsl:attribute name="font-family">Arial</xsl:attribute>
    <xsl:attribute name="font-size">6pt</xsl:attribute>
    <xsl:attribute name="font-weight">bold</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="fieldDataSmall">
    <xsl:attribute name="font-family">Arial</xsl:attribute>
    <xsl:attribute name="font-size">5pt</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="paddingTB">
    <xsl:attribute name="padding-before">1mm</xsl:attribute>
    <xsl:attribute name="padding-after">1mm</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="border">
    <xsl:attribute name="border">#000000 0.5pt solid</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="borderT">
    <xsl:attribute name="border-top">#000000 0.5pt solid</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="borderB">
    <xsl:attribute name="border-bottom">#000000 0.5pt solid</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="borderL">
    <xsl:attribute name="border-left">#000000 0.5pt solid</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="borderR">
    <xsl:attribute name="border-right">#000000 0.5pt solid</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="borderLR">
    <xsl:attribute name="border-left">#000000 0.5pt solid</xsl:attribute>
    <xsl:attribute name="border-right">#000000 0.5pt solid</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="borderBLR">
    <xsl:attribute name="border-bottom">#000000 0.5pt solid</xsl:attribute>
    <xsl:attribute name="border-left">#000000 0.5pt solid</xsl:attribute>
    <xsl:attribute name="border-right">#000000 0.5pt solid</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="headerCenterColumn">
    <xsl:attribute name="border-left">#3E90CD 0.5pt solid</xsl:attribute>
    <xsl:attribute name="border-right">#3E90CD 0.5pt solid</xsl:attribute>
    <xsl:attribute name="background-color">#D7E9F8</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="doubleLineDivider">
    <xsl:attribute name="border-top">#103F74 .01mm solid</xsl:attribute>
    <xsl:attribute name="border-bottom">#103F74 .01mm solid</xsl:attribute>
    <xsl:attribute name="height">.5mm</xsl:attribute>
  </xsl:attribute-set>
  <xsl:attribute-set name="pageWaterMark">
    <xsl:attribute name="font-family">Calibri</xsl:attribute>
    <xsl:attribute name="font-size">80pt</xsl:attribute>
    <xsl:attribute name="font-weight">bold</xsl:attribute>
    <xsl:attribute name="color">#103F74</xsl:attribute>
    <xsl:attribute name="text-align">center</xsl:attribute>
    <xsl:attribute name="display-align">center</xsl:attribute>
  </xsl:attribute-set>
</xsl:stylesheet>

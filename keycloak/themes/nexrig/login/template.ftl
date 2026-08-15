<#macro registrationLayout bodyClass="" displayInfo=false displayMessage=true displayRequiredFields=false>
<!doctype html>
<html lang="${lang!'en'}"<#if realm.internationalizationEnabled && locale??> dir="${(locale.rtl)?then('rtl','ltr')}"</#if>>
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <meta name="color-scheme" content="light">
  <title>${msg("loginTitle", (realm.displayName!"NEXRIG"))}</title>
  <#if properties.styles?has_content>
    <#list properties.styles?split(' ') as style>
      <link href="${url.resourcesPath}/${style}" rel="stylesheet">
    </#list>
  </#if>
  <#if scripts??>
    <#list scripts as script><script src="${script}" type="text/javascript"></script></#list>
  </#if>
</head>
<body class="${bodyClass}" data-page-id="login-${pageId}">
  <main class="login-page">
    <section class="login-panel">
      <a class="login-back" href="${(client.baseUrl)!'http://localhost:5173/'}" aria-label="Back to computer store">
        <span aria-hidden="true">&larr;</span> Back to computer store
      </a>
      <div class="login-card">
        <div class="brand-row">
          <img class="brand-mark" src="${url.resourcesPath}/img/nexrig-mark.svg" alt="" aria-hidden="true">
          <strong class="brand-name">NEXRIG</strong>
          <span class="development-pill">Secure access</span>
        </div>

        <h1 id="kc-page-title"><#if pageId == "login">Welcome back<#else><#nested "header"></#if></h1>
        <#if pageId == "login-reset-password">
          <p class="page-intro">Enter your username or email and we will send you a secure reset link.</p>
        <#elseif pageId == "login-update-password">
          <p class="page-intro">Choose a new password for your NEXRIG account.</p>
        <#elseif pageId == "register">
          <p class="page-intro">Create your NEXRIG customer account to save items, shop, and track orders.</p>
        <#else>
          <p class="page-intro">Sign in to continue to your NEXRIG account.</p>
        </#if>

        <#if displayMessage && message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
          <div class="alert alert-${message.type}" role="alert">${kcSanitize(message.summary)?no_esc}</div>
        </#if>

        <#if auth?has_content && auth.showUsername() && !auth.showResetCredentials()>
          <#nested "show-username">
        </#if>
        <#nested "form">
        <#nested "socialProviders">

        <#if displayInfo>
          <div id="kc-info" class="login-info"><#nested "info"></div>
        </#if>

        <#if pageId == "login">
          <div class="demo-hint">
            <strong>Local accounts</strong>
            <div><span>Customer</span><code>user / 1234</code></div>
            <div><span>Administrator</span><code>admin / 1234</code></div>
          </div>
          <small class="security-note">Authentication is handled by Keycloak using OpenID Connect and PKCE.</small>
        </#if>
      </div>
    </section>

    <aside class="login-visual" aria-label="NEXRIG welcome">
      <div>
        <span class="eyebrow">One computer store, two views</span>
        <h2>Shop simply.<br>Operate clearly.</h2>
        <p>The customer and admin experiences share one live backend&mdash;with secure, centralized account access.</p>
      </div>
    </aside>
  </main>
  <script type="module" src="${url.resourcesPath}/js/passwordVisibility.js"></script>
</body>
</html>
</#macro>

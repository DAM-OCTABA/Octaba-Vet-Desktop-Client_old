Imports System.IdentityModel.Tokens.Jwt
Imports Flurl.Http
Imports Flurl



Public Class frmLogin
    Private Property DeviceCode As String
    Public Async Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        btnLogin.Visible = False
        Dim request = Await "https://octaba.eu.auth0.com" _
            .AppendPathSegments(New String() {"oauth", "device", "code"}) _
            .PostUrlEncodedAsync(New With {
                .client_id = "c03PqrkcMVcBud4cPMjHAT4qJpywtzje",
                .scope = "openid profile"
            })
        Dim result = Await request.GetJsonAsync(Of DeviceResponse)()
        Dim psi = New ProcessStartInfo With {
            .FileName = result.verification_uri_complete,
            .UseShellExecute = True
            }
        Process.Start(psi)
        DeviceCode = result.device_code
        time.Interval = result.interval * 1000
        time.Start()
        Message1.Visible = True

        TextBox1.Text = Str(Sum(1, 2))
    End Sub

    Public Function Sum(a As Integer, b As Integer) As Integer
        Return a + b
    End Function

    Public Async Sub timeTick(sender As Object, e As EventArgs) Handles time.Tick
        Dim req = Await "https://octaba.eu.auth0.com" _
            .AppendPathSegments(New String() {"oauth", "token"}) _
            .AllowAnyHttpStatus() _
            .PostUrlEncodedAsync(New With {
                .grant_type = "urn:ietf:params:oauth:grant-type:device_code",
                .device_code = DeviceCode,
                .client_id = "c03PqrkcMVcBud4cPMjHAT4qJpywtzje"
            })
        If req.StatusCode <= 299 Then
            Dim res = Await req.GetJsonAsync(Of TokenResponse)()
            Dim token = New JwtSecurityToken(jwtEncodedString:=res.id_token)
            Message1.Visible = False
            'Message2.Text = $"Has iniciat sesisió amb l'usuari:, {token.Claims.FirstOrDefault(Function(c) c.Type = JwtRegisteredClaimNames.Name)?.Value}"
            'Message2.Visible = True
            frmMenu.Show()
            Me.Hide()
        End If
    End Sub

    Class DeviceResponse
        Public Property device_code As String
        Public Property user_code As String
        Public Property verification_uri As String
        Public Property verification_uri_complete As String
        Public Property expires_in As Integer
        Public Property interval As Integer

    End Class

    Class TokenResponse
        Public Property access_token As String
        Public Property id_token As String
        Public Property refresh_token As String
        Public Property token_type As String
        Public Property expires_in As Integer
    End Class

    Private Sub frmLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class

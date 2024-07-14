using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Interfaces;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;
using TMPro;

[Table("profiles")]
class Profile : BaseModel
{
    [PrimaryKey("id")]
    public string id { get; set; }

    [Column("updated_at")]
    public string UpdatedAt { get; set; }

    [Column("username")]
    public string UserName { get; set; }

    [Column("full_name")]
    public string FullName { get; set; }

    [Column("avatar_url")]
    public string AvatarURL { get; set; }

    [Column("character_name")]
    public string CharacterName { get; set; }

    [Column("model_url")]
    public string ModelURL { get; set; }
}

public class Login : MonoBehaviour
{
    [SerializeField] TMP_InputField Field;
    [SerializeField] TMP_Text StatusText;
    [SerializeField] TMP_Text ButtonText;

    private bool didSendMagicLink = false;
    private string email;

    public async void OnClick()
    {
        //supabaseのクライアントを初期化
        var supabase = new Supabase.Client(API_KEY.SUPABASE_URL, API_KEY.SUPABASE_ANON_KEY);
        await supabase.InitializeAsync();
        string input = Field.GetComponent<TMP_InputField>().text;

        //OTP送信済みの場合OTPを認証してログインする
        if (didSendMagicLink)
        {
            var session = await supabase.Auth.VerifyOTP(email, input, Supabase.Gotrue.Constants.EmailOtpType.MagicLink);
            var id = supabase.Auth.CurrentUser.Id;

            Debug.Log(supabase.Auth.CurrentUser);
            StatusText.SetText($"Your ID:{id}");

            //ユーザーデータを取得
            var result = await supabase.From<Profile>()
                .Where(user => user.id == id)
                .Single();

            Debug.Log(result.CharacterName);
        }
        else
        {
            //OTPを送信
            email = input;
            await supabase.Auth.SendMagicLink(email);
            ButtonText.SetText("Verify OTP");
            didSendMagicLink = true;

            Field.text = "";
        }
    }
}

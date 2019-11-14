using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RegistryManagementV3.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<SelectList> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ResourceViewModel
    {
        public int? Id { get; set; }
        [Required] 
        [Display(Name = "Назва")] 
        public string Title { get; set; }
        
        [Required] 
        [Display(Name = "Опис")] 
        public string Description { get; set; }
        
        [Required] 
        [Display(Name = "Мова")] 
        public string Language { get; set; }

        [DefaultValue(5)]
        [Range(1, 10)]
        [Display(Name = "Рівень доступу")]
        public int SecurityLevel { get; set; }

        [Required]
        [Display(Name="Файл")]
        public IFormFile ResourceFile { get; set; }

        [DefaultValue(5)]
        [Range(1, 10)]
        [Display(Name = "Пріоритет")]
        public int? Priority { get; set; }

        public long? CatalogId { get; set; }

        [Display(Name = "Теги")]
        public virtual string Tags { get; set; }
    }

    public class UpdateResourceViewModel
    {
        public int? Id { get; set; }
        [Required]
        [Display(Name = "Назва")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Опис")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Мова")]
        public string Language { get; set; }

        [Required]
        [Display(Name = "Формат")]
        public string Format { get; set; }

        [DefaultValue(5)]
        [Range(1, 10)]
        [Display(Name = "Рівень доступу")]
        public int SecurityLevel { get; set; }

        [Display(Name = "Файл")]
        public IFormFile ResourceFile { get; set; }

        [Display(Name = "Пріоритет")]
        public int? Priority { get; set; }

        public int? CatalogId { get; set; }

        [Display(Name = "Теги")]
        public virtual string Tags { get; set; }
    }

    public class CatalogViewModel
    {
        public long?  Id { get; set; }
        public long? CatalogId { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [DefaultValue(5)]
        [Range(1, 10)]
        [Display(Name = "Рівень доступу")]
        public int SecurityLevel { get; set; }
        [Display(Name = "Groups")]
        public string? Groups { get; set; }
    }

    public class SearchViewModel
    {
        [Required]
        public string Query { get; set; }
    }
    public class SearchFilterViewModel
    {
        public string Query { get; set; }
        public string CreationDateRange { get; set; }
        public string ApprovalDateRange { get; set; }
        public string Author { get; set; }
        public string Tags { get; set; }
        public string OrderBy { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Імя користувача")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запамятати мене?")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class RegisterViewModel
     {
        [Required]
        [Display(Name = "Імя")]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name = "Емейл")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Пароль {0} повинен містити хоча б {2} символи.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердити пароль")]
        [Compare("Password", ErrorMessage = "Пароль і пароль для підтвердження повинні співпадати.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Емейл")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Пароль {0} повинен містити хоча б {2} символи.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердити пароль")]
        [Compare("Password", ErrorMessage = "Обидва паролі повинні співпадати")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ApplicationUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Статус акаунта")]
        public string AccountStatus { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Емейл")]
        public string Email { get; set; }

        [Display(Name = "Номер телефону")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Кількість невдалих спроб авторизація")]
        public int AccessFailedCount { get; set; }

        [Required]
        [Display(Name = "Імя")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Група")]
        public string UserGroup { get; set; }
    }
}

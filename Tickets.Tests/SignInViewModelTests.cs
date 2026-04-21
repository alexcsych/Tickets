using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tickets.Models;
using Tickets.ViewModels;

namespace Tickets.Tests
{
    public class SignUpViewModelTests
    {
        private static void SetFieldTouched(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(obj, true);
        }

        [Fact]
        public void Email_ShouldReturnError_WhenAtSymbolIsMissing()
        {
            var vm = new SignUpViewModel(null!);
            SetFieldTouched(vm, "_isEmailTouched");

            vm.Email = "oleksii.sych.com";
            string error = vm["Email"];

            Assert.Equal("Невірний формат пошти!", error);
        }

        [Fact]
        public void Password_ShouldReturnError_WhenTooShort()
        {
            var vm = new SignUpViewModel(null!);
            SetFieldTouched(vm, "_isPasswordTouched");

            vm.Password = "123";
            string error = vm["Password"];

            Assert.Equal("Мін. 6 символів!", error);
        }

        [Fact]
        public void ConfirmPassword_ShouldReturnError_WhenNotEqualPassword()
        {
            var vm = new SignUpViewModel(null!);
            SetFieldTouched(vm, "_isPasswordTouched");
            SetFieldTouched(vm, "_isConfirmPasswordTouched");

            vm.Password = "StrongPassword123";
            vm.ConfirmPassword = "DifferentPassword123";
            string error = vm["ConfirmPassword"];

            Assert.Equal("Паролі не співпадають!", error);
        }

        [Fact]
        public void Name_ShouldReturnEmpty_WhenValid()
        {
            var vm = new SignUpViewModel(null!);
            SetFieldTouched(vm, "_isNameTouched");

            vm.Name = "Олексій";
            string error = vm["Name"];

            Assert.Equal(string.Empty, error);
        }
    }
}

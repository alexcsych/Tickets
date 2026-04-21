using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tickets.ViewModels;

namespace Tickets.Tests
{
    public class LogInViewModelTests
    {
        private void SetFieldTouched(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(obj, true);
        }

        [Fact]
        public void Email_ShouldReturnError_WhenTooShort()
        {
            var vm = new LogInViewModel(null!);
            SetFieldTouched(vm, "_isEmailTouched");

            vm.Email = "abc";
            string error = vm["Email"];

            Assert.Equal("Пошта занадто коротка (мін. 5)!", error);
        }

        [Fact]
        public void Password_ShouldReturnError_WhenEmpty()
        {
            var vm = new LogInViewModel(null!);
            SetFieldTouched(vm, "_isPasswordTouched");

            vm.Password = "   ";
            string error = vm["Password"];

            Assert.Equal("Пароль не може бути порожнім!", error);
        }

        [Fact]
        public void Password_ShouldReturnError_WhenTooShort()
        {
            var vm = new LogInViewModel(null!);
            SetFieldTouched(vm, "_isPasswordTouched");

            vm.Password = "12345";
            string error = vm["Password"];

            Assert.Equal("Пароль занадто короткий (мін. 6)!", error);
        }

        [Fact]
        public void Validation_ShouldReturnEmpty_WhenDataIsValid()
        {
            var vm = new LogInViewModel(null!);
            SetFieldTouched(vm, "_isEmailTouched");
            SetFieldTouched(vm, "_isPasswordTouched");

            vm.Email = "admin@test.com";
            vm.Password = "password123";

            Assert.Equal(string.Empty, vm["Email"]);
            Assert.Equal(string.Empty, vm["Password"]);
        }
    }
}

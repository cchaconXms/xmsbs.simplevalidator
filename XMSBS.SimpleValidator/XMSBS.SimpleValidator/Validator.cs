﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XMSBS.SimpleValidator.Exceptions;
using XMSBS.SimpleValidator.Interfaces;

namespace XMSBS.SimpleValidator
{
    public partial class Validator<T>
    {
        private List<string> DataValidationValidators { get; set; }
        private List<string> BussinesRulesValidators { get; set; }
        private T Obj { get; set; }


        public Validator(T obj)
        {
            this.DataValidationValidators = new List<string>();
            this.BussinesRulesValidators = new List<string>();
            this.Obj = obj;
        }

        public Validator<T> IsNotNullOrEmpty(Func<T, string> func, string errorMessage)
        {
            if (string.IsNullOrEmpty(func.Invoke(Obj)))
                DataValidationValidators.Add(errorMessage);
            return this;
        }

        public Validator<T> IsNotNullOrWhiteSpace(Func<T, string> func, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(func.Invoke(Obj)))
                DataValidationValidators.Add(errorMessage);
            return this;
        }

        public Validator<T> IsMaxLength(Func<T, string> func, int max, string errorMessage)
        {
            if (!(func.Invoke(Obj).Length == max))
                DataValidationValidators.Add(errorMessage);
            return this;
        }

        public Validator<T> IsMinLength(Func<T, string> func, int min, string errorMessage)
        {
            if (!(func.Invoke(Obj).Length == min))
                DataValidationValidators.Add(errorMessage);
            return this;
        }

        public Validator<T> IsEmail(Func<T, string> func, string errorMessage)
        {
            string exp = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$";
            if (!new Regex(exp, RegexOptions.IgnoreCase).IsMatch(func.Invoke(Obj)))
                DataValidationValidators.Add(errorMessage);
            return this;
        }

        public Validator<T> IsPassword(Func<T, string> func, string errorMessage)
        {
            string exp = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,30}$";
            if (!new Regex(exp, RegexOptions.IgnoreCase).IsMatch(func.Invoke(Obj)))
                DataValidationValidators.Add(errorMessage);
            return this;
        }

        public Validator<T> IsRegex(Func<T, string> func, string exp, string errorMessage)
        {
            if (!new Regex(exp, RegexOptions.IgnoreCase).IsMatch(func.Invoke(Obj)))
                DataValidationValidators.Add(errorMessage);
            return this;
        }

        public Validator<T> IsNotZero(Func<T, int> func, string errorMessage)
        {
            if (func.Invoke(Obj) == 0)
                DataValidationValidators.Add(errorMessage);
            return this;
        }

        public Validator<T> Is(Func<T, int> func, int compare, string errorMessage)
        {
            if (func.Invoke(Obj) != compare)
                DataValidationValidators.Add(errorMessage);
            return this;
        }

        public Validator<T> IsGreaterThan(Func<T, int> func, int min, string errorMessage)
        {
            if (!(func.Invoke(Obj) >= min))
                DataValidationValidators.Add(errorMessage);
            return this;
        }

        public Validator<T> IsLessThan(Func<T, int> func, int max, string errorMessage)
        {
            if (!(func.Invoke(Obj) <= max))
                DataValidationValidators.Add(errorMessage);
            return this;
        }

        public Validator<T> IsBetween(Func<T, int> func, int min, int max, string errorMessage)
        {
            if (!(func.Invoke(Obj) <= max && func.Invoke(Obj) >= min))
                DataValidationValidators.Add(errorMessage);
            return this;
        }

        public Validator<T> AddDataValidation(Func<T, string> func)
        {
            if (!string.IsNullOrEmpty(func.Invoke(Obj)))
                DataValidationValidators.Add(func.Invoke(Obj));
            return this;
        }

        public async Task<Validator<T>> AddBussinesRules(IRule rule)
        {
            var result = await rule.ExecuteAsync();

            if (!result.IsSuccess)
            {
                if (result.Exception.Messages.Any())
                    BussinesRulesValidators.AddRange(result.Exception.Messages);
            }

            return this;
        }

        public Validator<T> ExecuteBussinesRulesValidation()
        {
            var result = new List<string>();

            if (this.BussinesRulesValidators.Any())
            {
                this.BussinesRulesValidators.ForEach(c =>
                {
                    if (!string.IsNullOrEmpty(c)) result.Add(c);
                });

                if (result.Any()) throw new BusinessException(result.ToArray());
            }

            return this;
        }

        public Validator<T> ExecuteDataValidations()
        {
            var result = new List<string>();

            if (this.DataValidationValidators.Any())
            {
                this.DataValidationValidators.ForEach(c =>
                {
                    if (!string.IsNullOrEmpty(c)) result.Add(c);
                });

                if (result.Any()) throw new DataValidationException(result.ToArray());
            }

            return this;
        }

    }
}

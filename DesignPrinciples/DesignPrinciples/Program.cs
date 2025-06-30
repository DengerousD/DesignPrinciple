using System;

namespace DesignPrinciples
{
    
  
    #region 1. 单一职责原则（SRP）
    // 负责用户数据校验，只验证用户名和密码的有效性。
    public interface IUserValidator
    {
        bool Validate(string? username, string? password);
    }

    public class BasicUserValidator : IUserValidator
    {
        public bool Validate(string? username, string? password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("用户名不能为空");
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("密码不能为空");
                return false;
            }
            Console.WriteLine("基本校验通过");
            return true;
        }
    }

    // 负责用户注册，只负责存储新用户信息。
    public interface IUserRegistration
    {
        void Register(string username);
    }

    public class UserRegistration : IUserRegistration
    {
        public void Register(string username)
        {
            Console.WriteLine($"用户 {username} 注册成功");
            // 模拟数据库写入等操作
        }
    }
    #endregion

    #region 2. 开闭原则（OCP）
    // 扩展校验规则的接口
    public interface IValidationRule
    {
        bool Validate(string? username, string? password);
    }

    // 校验用户名长度
    public class UsernameLengthRule : IValidationRule
    {
        public bool Validate(string? username, string? password)
        {
            if (username == null || username.Length < 3)
            {
                Console.WriteLine("用户名长度不能少于3个字符");
                return false;
            }
            return true;
        }
    }

    // 校验密码长度
    public class PasswordLengthRule : IValidationRule
    {
        public bool Validate(string? username, string? password)
        {
            if (password == null || password.Length < 6)
            {
                Console.WriteLine("密码长度不能少于6位");
                return false;
            }
            return true;
        }
    }

    // 组合多个校验规则，方便扩展
    public class CompositeValidator : IUserValidator
    {
        private readonly IValidationRule[] _rules;

        public CompositeValidator(params IValidationRule[] rules)
        {
            _rules = rules;
        }

        public bool Validate(string? username, string? password)
        {
            foreach (var rule in _rules)
            {
                if (!rule.Validate(username, password))
                    return false;
            }
            Console.WriteLine("高级校验通过");
            return true;
        }
    }
    #endregion

    #region 3. 里氏替换原则（LSP）
    // 基础用户验证器抽象
    public abstract class BaseValidator : IUserValidator
    {
        public abstract bool Validate(string? username, string? password);
    }

    // 简单用户名长度验证器，继承自BaseValidator
    public class SimpleUsernameValidator : BaseValidator
    {
        public override bool Validate(string? username, string? password)
        {
            if (username == null || username.Length < 3)
            {
                Console.WriteLine("用户名长度不能少于3个字符");
                return false;
            }
            return true;
        }
    }

    // 复合验证器，也继承BaseValidator，实现多规则校验
    public class AdvancedValidator : BaseValidator
    {
        private readonly IUserValidator _innerValidator;

        public AdvancedValidator(IUserValidator innerValidator)
        {
            _innerValidator = innerValidator;
        }

        public override bool Validate(string? username, string? password)
        {
            return _innerValidator.Validate(username, password);
        }
    }
    #endregion

    #region 4. 依赖倒置原则（DIP）
    // 抽象发动机接口
    public interface IEngine
    {
        void Start();
    }

    // 具体汽油发动机实现
    public class GasolineEngine : IEngine
    {
        public void Start() => Console.WriteLine("汽油发动机启动");
    }

    // 具体电动发动机实现
    public class ElectricEngine : IEngine
    {
        public void Start() => Console.WriteLine("电动发动机启动");
    }

    // 依赖抽象接口的高层模块Car
    public class Car
    {
        private readonly IEngine _engine;

        // 构造函数注入，传入具体发动机
        public Car(IEngine engine)
        {
            _engine = engine;
        }

        public void Run()
        {
            _engine.Start();
            Console.WriteLine("汽车运行中...");
        }
    }

    #endregion

    #region 5. 接口隔离原则（ISP）
    /// <summary>
    /// 用户权限校验接口
    /// </summary>
    public interface IUserPermissionValidator
    {
        bool HasPermission(string username);
    }

    /// <summary>
    /// 用户日志记录接口
    /// </summary>
    public interface IUserLogger
    {
        void LogAction(string username, string action);
    }

    public class UserPermissionValidator : IUserPermissionValidator
    {
        public bool HasPermission(string username)
        {
            // 模拟权限校验
            if (username.StartsWith("admin"))
            {
                Console.WriteLine($"用户 {username} 拥有管理员权限");
                return true;
            }

            Console.WriteLine($"用户 {username} 拥有普通权限");
            return false;
        }
    }

    public class UserLogger : IUserLogger
    {
        public void LogAction(string username, string action)
        {
            Console.WriteLine($"用户 {username} 执行了操作：{action}");
        }
    }
    #endregion

    // 统一管理用户注册的业务流程，依赖抽象接口，实现低耦合
    public class UserManager
    {
        private readonly IUserValidator _validator;
        private readonly IUserRegistration _registration;
        private readonly IUserPermissionValidator _permissionValidator;
        private readonly IUserLogger _logger;

        // 依赖注入接口，实现依赖倒置
        public UserManager(IUserValidator validator, IUserRegistration registration, IUserPermissionValidator permissionValidator, IUserLogger logger)
        {
            _validator = validator;
            _registration = registration;
            _permissionValidator = permissionValidator;
            _logger = logger;
        }                                   

        public void Register(string? username, string? password)
        {
            if (_validator.Validate(username, password))
            {
                if (_permissionValidator.HasPermission(username ?? ""))
                {
                    if (username != null)
                    {
                        _registration.Register(username);
                        _logger.LogAction(username, "注册成功");
                    }
                }
                else
                {
                    Console.WriteLine("权限不足，注册失败");
                }
            }
            else
            {
                Console.WriteLine("注册失败，用户数据不合法");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("请输入用户名：");
            string? username = Console.ReadLine();

            Console.WriteLine("请输入密码：");
            string? password = Console.ReadLine();

            // 创建接口实现类
            IUserValidator validator = new CompositeValidator(
                new UsernameLengthRule(),
                new PasswordLengthRule()
            );
            IUserRegistration registration = new UserRegistration();
            IUserPermissionValidator permissionValidator = new UserPermissionValidator();
            IUserLogger logger = new UserLogger();

            // 创建用户管理器，注入接口
            UserManager userManager = new UserManager(validator, registration, permissionValidator, logger);

            // 注册用户
            
            userManager.Register(username, password);
        }
    }
}

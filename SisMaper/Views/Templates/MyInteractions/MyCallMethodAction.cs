using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace SisMaper.Views.Templates.MyInteractions;

public class MyCallMethodAction : TriggerAction<DependencyObject>
{
    public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register("TargetObject",
        typeof(object), typeof(MyCallMethodAction), new PropertyMetadata(OnTargetObjectChanged));

    public static readonly DependencyProperty MethodNameProperty = DependencyProperty.Register("MethodName",
        typeof(string), typeof(MyCallMethodAction), new PropertyMetadata(OnMethodNameChanged));

    private List<MethodDescriptor> methodDescriptors;

    public MyCallMethodAction()
    {
        methodDescriptors = new List<MethodDescriptor>();
    }

    /// <summary>
    ///     The object that exposes the method of interest. This is a dependency property.
    /// </summary>
    public object TargetObject
    {
        get => GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    /// <summary>
    ///     The name of the method to invoke. This is a dependency property.
    /// </summary>
    public string MethodName
    {
        get => (string) GetValue(MethodNameProperty);
        set => SetValue(MethodNameProperty, value);
    }

    private object Target => TargetObject ?? AssociatedObject;

    /// <summary>
    ///     Invokes the action.
    /// </summary>
    /// <param name="parameter">
    ///     The parameter of the action. If the action does not require a parameter, the parameter may be
    ///     set to a null reference.
    /// </param>
    ///// <exception cref="ArgumentException">A method with <c cref="MethodName"/> could not be found on the <c cref="TargetObject"/>.</exception>
    protected override void Invoke(object parameter)
    {
        if (AssociatedObject != null)
        {
            MethodDescriptor methodDescriptor = FindBestMethod(parameter);
            if (methodDescriptor != null)
            {
                ParameterInfo[] parameters = methodDescriptor.Parameters;

                if (parameters.Length == 0)
                {
                    methodDescriptor.MethodInfo.Invoke(Target, null);
                }
                else if (parameters.Length == 2 && AssociatedObject != null && parameter != null)
                {
                    if (parameters[0].ParameterType.IsAssignableFrom(AssociatedObject.GetType())
                        && parameters[1].ParameterType.IsAssignableFrom(parameter.GetType()))
                    {
                        methodDescriptor.MethodInfo.Invoke(Target, new[] {AssociatedObject, parameter});
                    }
                }
            }
            else if (TargetObject != null) { }
        }
    }

    /// <summary>
    ///     Called after the action is attached to an AssociatedObject.
    /// </summary>
    /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
    protected override void OnAttached()
    {
        base.OnAttached();
        UpdateMethodInfo();
    }

    /// <summary>
    ///     Called when the action is getting detached from its AssociatedObject, but before it has actually occurred.
    /// </summary>
    /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
    protected override void OnDetaching()
    {
        methodDescriptors.Clear();
        base.OnDetaching();
    }

    private MethodDescriptor FindBestMethod(object parameter)
    {
        Type parameterType = parameter == null ? null : parameter.GetType();

        return methodDescriptors.FirstOrDefault(methodDescriptor =>
        {
            return !methodDescriptor.HasParameters ||
                   parameter != null &&
                   methodDescriptor.SecondParameterType.IsAssignableFrom(parameter.GetType());
        });
    }

    private void UpdateMethodInfo()
    {
        methodDescriptors.Clear();

        if (Target != null && !string.IsNullOrEmpty(MethodName))
        {
            Type targetType = Target.GetType();
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];

                if (!IsMethodValid(method))
                {
                    continue;
                }

                ParameterInfo[] methodParams = method.GetParameters();

                if (!AreMethodParamsValid(methodParams))
                {
                    continue;
                }

                methodDescriptors.Add(new MethodDescriptor(method, methodParams));
            }

            methodDescriptors = methodDescriptors.OrderByDescending(methodDescriptor =>
            {
                int distanceFromBaseClass = 0;

                if (methodDescriptor.HasParameters)
                {
                    Type typeWalker = methodDescriptor.SecondParameterType;
                    while (typeWalker != typeof(EventArgs))
                    {
                        distanceFromBaseClass++;
                        typeWalker = typeWalker.BaseType;
                    }
                }

                return methodDescriptor.ParameterCount + distanceFromBaseClass;
            }).ToList();
        }
    }

    private bool IsMethodValid(MethodInfo method)
    {
        if (!string.Equals(method.Name, MethodName, StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }

    private static bool AreMethodParamsValid(ParameterInfo[] methodParams)
    {
        if (methodParams.Length == 2)
        {
            if (methodParams[0].ParameterType != typeof(object))
            {
                return false;
            }

            if (!typeof(EventArgs).IsAssignableFrom(methodParams[1].ParameterType))
            {
                return false;
            }
        }
        else if (methodParams.Length != 0)
        {
            return false;
        }

        return true;
    }

    private static void OnMethodNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        MyCallMethodAction MyCallMethodAction = (MyCallMethodAction) sender;
        MyCallMethodAction.UpdateMethodInfo();
    }

    private static void OnTargetObjectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        MyCallMethodAction MyCallMethodAction = (MyCallMethodAction) sender;
        MyCallMethodAction.UpdateMethodInfo();
    }

    private class MethodDescriptor
    {
        public MethodDescriptor(MethodInfo methodInfo, ParameterInfo[] methodParams)
        {
            MethodInfo = methodInfo;
            Parameters = methodParams;
        }

        public MethodInfo MethodInfo { get; }

        public bool HasParameters => Parameters.Length > 0;

        public int ParameterCount => Parameters.Length;

        public ParameterInfo[] Parameters { get; }

        public Type SecondParameterType
        {
            get
            {
                if (Parameters.Length >= 2)
                {
                    return Parameters[1].ParameterType;
                }

                return null;
            }
        }
    }
}
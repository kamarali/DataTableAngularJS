﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 

namespace Iata.IS.Model.Calendar
{
  /// <remarks/>
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [Serializable]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false)]
  public class Trigger
  {
    private string _cronExpressionField;
    private string _jobNameField;
    private string _nameField;

    /// <remarks/>
    [XmlAttribute]
    public string Name
    {
      get
      {
        return _nameField;
      }
      set
      {
        _nameField = value;
      }
    }

    /// <remarks/>
    [XmlAttribute]
    public string JobName
    {
      get
      {
        return _jobNameField;
      }
      set
      {
        _jobNameField = value;
      }
    }

    /// <remarks/>
    [XmlAttribute]
    public string CronExpression
    {
      get
      {
        return _cronExpressionField;
      }
      set
      {
        _cronExpressionField = value;
      }
    }
  }

  /// <remarks/>
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [Serializable]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false)]
  public class ISCalendarTriggers
  {
    private ISCalendarTriggersCronTriggers[] _cronTriggersField;
    private ISCalendarTriggersSimpleTriggers[] _simpleTriggersField;

    /// <remarks/>
    [XmlElement("SimpleTriggers", typeof(ISCalendarTriggersSimpleTriggers), Form = XmlSchemaForm.Unqualified)]
    public ISCalendarTriggersSimpleTriggers[] SimpleTriggers
    {
      get
      {
        return _simpleTriggersField;
      }
      set
      {
        _simpleTriggersField = value;
      }
    }

    /// <remarks/>
    [XmlElement("CronTriggers", typeof(ISCalendarTriggersCronTriggers), Form = XmlSchemaForm.Unqualified)]
    public ISCalendarTriggersCronTriggers[] CronTriggers
    {
      get
      {
        return _cronTriggersField;
      }
      set
      {
        _cronTriggersField = value;
      }
    }
  }

  /// <remarks/>
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [Serializable]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class ISCalendarTriggersCronTriggers
  {
    private Trigger[] _triggerField;

    /// <remarks/>
    [XmlElement("Trigger")]
    public Trigger[] Trigger
    {
      get
      {
        return _triggerField;
      }
      set
      {
        _triggerField = value;
      }
    }
  }

  /// <remarks/>
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [Serializable]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class ISCalendarTriggersSimpleTriggers
  {
    private Trigger[] _triggerField;

    /// <remarks/>
    [XmlElement("Trigger")]
    public Trigger[] Trigger
    {
      get
      {
        return _triggerField;
      }
      set
      {
        _triggerField = value;
      }
    }
  }
}
Option Explicit On
Option Strict On

Imports System.Reflection
Imports System.Xml
Imports NAnt.Core
Imports NAnt.Core.Attributes
Imports System.Globalization

<ElementName("items")> _
Public Class LoopItemContainer
    Inherits DataTypeBase
    Implements IEnumerable

    Private _Items As LoopItems

    <BuildElement("loopitem", Required:=True)> _
    Public Property Items() As LoopItems
        Get
            Return _Items
        End Get
        Set(ByVal value As LoopItems)
            _Items = value
        End Set
    End Property

    Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return Me.Items.GetEnumerator
    End Function

    Protected Overrides Sub InitializeXml(ByVal elementNode As System.Xml.XmlNode, ByVal properties As NAnt.Core.PropertyDictionary, ByVal framework As NAnt.Core.FrameworkInfo)
        Me.XmlNode = elementNode
        Dim configurator As New LoopItemsConfigurator(Me, elementNode, properties, framework)
        configurator.Initialize()
    End Sub

    Private Class LoopItemsConfigurator
        Inherits NAnt.Core.Element.AttributeConfigurator

        Public Sub New(ByVal element As Element, ByVal elementNode As XmlNode, ByVal properties As PropertyDictionary, ByVal targetFramework As FrameworkInfo)
            MyBase.New(element, elementNode, properties, targetFramework)
        End Sub

        Protected Overrides Function InitializeChildElement(ByVal propertyInfo As System.Reflection.PropertyInfo) As Boolean
            Dim CurrentNode As XmlNode
            Dim objArray1 As Object()
            Dim attribute1 As BuildElementAttribute = CType(Attribute.GetCustomAttribute(propertyInfo, GetType(BuildElementAttribute), False), BuildElementAttribute)
            If (attribute1 Is Nothing) Then
                Return False
            End If
            If (Me.ElementXml.OwnerDocument.DocumentElement Is Nothing) Then
                CurrentNode = Me.ElementXml.Item(attribute1.Name)
            Else
                CurrentNode = Me.ElementXml.Item(attribute1.Name, Me.ElementXml.OwnerDocument.DocumentElement.NamespaceURI)
            End If
            If propertyInfo.Name = "Items" Then
                For Each Builder As DataTypeBaseBuilder In TypeFactory.DataTypeBuilders
                    CurrentNode = Me.ElementXml.Item(Builder.DataTypeName)
                    If Not CurrentNode Is Nothing Then
                        Dim SubjectAssembly As System.Reflection.Assembly = System.Reflection.Assembly.LoadFrom(Builder.AssemblyFileName)
                        If SubjectAssembly.GetType(Builder.ClassName).IsSubclassOf(GetType(LoopItems)) Then
                            Exit For
                        End If
                        CurrentNode = Nothing
                    End If
                Next
            End If
            If ((CurrentNode Is Nothing) AndAlso attribute1.Required) Then
                objArray1 = New Object() {attribute1.Name, Me.Name}
                Throw New BuildException(String.Format(CultureInfo.InvariantCulture, NAnt.Core.Util.ResourceUtils.GetString("NA1013"), objArray1), Me.Location)
            End If
            If (Not CurrentNode Is Nothing) Then
                Me.UnprocessedChildNodes.Remove(CurrentNode.Name)
                If Not attribute1.ProcessXml Then
                    Return True
                End If
                Me.CreateChildBuildElement(propertyInfo, CurrentNode, Me.Properties, Me.TargetFramework)
                If (Me.ElementXml.SelectNodes(("nant:" & attribute1.Name), Me.NamespaceManager).Count > 1) Then
                    objArray1 = New Object() {Me.Name, attribute1.Name}
                    Throw New BuildException(String.Format(CultureInfo.InvariantCulture, "<{0} ... /> does not support multiple '{1}' child elements.", objArray1), Me.Location)
                End If
            End If
            Return True
        End Function

        Private Shadows Function CreateChildBuildElement(ByVal propInfo As PropertyInfo, ByVal xml As XmlNode, ByVal properties As PropertyDictionary, ByVal framework As FrameworkInfo) As Element
            Dim getter As MethodInfo = Nothing
            Dim setter As MethodInfo = Nothing
            Dim childElement As Element = Nothing
            Dim elementType As Type = Nothing

            setter = propInfo.GetSetMethod(True)
            getter = propInfo.GetGetMethod(True)

            'if there is a getter, then get the current instance of the object, and use that
            If Not getter Is Nothing Then

                Try
                    childElement = DirectCast(propInfo.GetValue(Element, Nothing), Element)
                Catch ex As InvalidCastException
                    Throw New BuildException(String.Format("Property ""{0}"" for class ""{1}"" is backed by ""{2}"" which does not derive from ""{3}"".", _
                        propInfo.Name, Element.GetType().FullName, propInfo.PropertyType.FullName, _
                        GetType(Element).FullName))
                End Try
                If childElement Is Nothing Then
                    If setter Is Nothing Then
                        Throw New BuildException(String.Format("Property {0} cannot return null (if there is no set method) for class {1}", propInfo.Name, _
                                Element.GetType().FullName), Location)
                    Else
                        ' fake the getter as null so we process the rest like there is no getter
                        getter = Nothing
                        Me.Project.Log(Level.Debug, String.Format("{0}_get() returned null; will go the route of set method to populate.", propInfo.Name))
                    End If
                Else
                    elementType = childElement.GetType()
                End If
            End If

            ' create a new instance of the object if there is not a get method. (or the get object returned null... see above)
            If getter Is Nothing AndAlso Not setter Is Nothing Then
                elementType = setter.GetParameters()(0).ParameterType
                If elementType.IsAbstract Then
                    Dim Builder As DataTypeBaseBuilder
                    Builder = TypeFactory.DataTypeBuilders.Item(xml.Name)
                    If Builder Is Nothing Then
                        Throw New InvalidOperationException(String.Format("Abstract type: {0} for {2}.{1}", elementType.Name, propInfo.Name, Name))
                    End If
                    childElement = Builder.CreateDataTypeBase()
                Else
                    childElement = DirectCast(Activator.CreateInstance(elementType, (BindingFlags.NonPublic Or (BindingFlags.Public Or BindingFlags.Instance)), Nothing, Nothing, CultureInfo.InvariantCulture), Element)
                End If
            End If

            ' initialize the child element
            childElement = NAnt.Core.Element.InitializeBuildElement(Element, xml, childElement, elementType)

            ' check if we're dealing with a reference to a data type
            Dim dataType As DataTypeBase = DirectCast(childElement, DataTypeBase)
            If Not dataType Is Nothing AndAlso Not xml.Attributes("refid") Is Nothing Then
                ' references to data type should be always be set
                If setter Is Nothing Then
                    Throw New BuildException(String.Format("DataType child element '{0}' in class '{1}' must define a set method.", _
                            propInfo.Name, Me.GetType().FullName))
                End If
                ' re-set the getter (for force the setter to be used)
                getter = Nothing
            End If

            ' call the set method if we created the object
            If Not setter Is Nothing AndAlso getter Is Nothing Then
                setter.Invoke(Element, New Object() {childElement})
            End If

            ' return the new/used object
            Return childElement
        End Function

    End Class

End Class

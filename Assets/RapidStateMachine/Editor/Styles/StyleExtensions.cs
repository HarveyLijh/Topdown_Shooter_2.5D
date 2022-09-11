using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RSM
{
    public static class StyleExtensions
    {
        public static void SetBorderRadius(this IStyle thisStyle, float radius)
        {
            thisStyle.borderBottomLeftRadius = radius;
            thisStyle.borderTopLeftRadius = radius;
            thisStyle.borderTopRightRadius = radius;
            thisStyle.borderBottomRightRadius = radius;
        }
        public static void SetPadding(this IStyle thisStyle, float padding)
        {
            thisStyle.paddingBottom = padding;
            thisStyle.paddingLeft = padding;
            thisStyle.paddingRight = padding;
            thisStyle.paddingTop = padding;
        }

        public static void SetBorderColour(this IStyle thisStyle, Color colour)
        {
            thisStyle.borderBottomColor = colour;
            thisStyle.borderTopColor = colour;
            thisStyle.borderLeftColor = colour;
            thisStyle.borderRightColor = colour;
        }

        public static void SetSliceSize(this IStyle thisStyle, int size)
        {
            thisStyle.unitySliceBottom = size;
            thisStyle.unitySliceTop = size;
            thisStyle.unitySliceLeft = size;
            thisStyle.unitySliceRight = size;
        }

        public static void SetSize(this IStyle thisStyle, Vector2 size)
        {
            thisStyle.width = size.x;
            thisStyle.height = size.y;
        }
        public static void SetIcon(this IStyle thisStyle, Texture2D icon, float size = 0, int slice = 0)
        {
            thisStyle.backgroundImage = icon;
            thisStyle.backgroundColor = Color.clear;
            thisStyle.SetBorderColour(Color.clear);
            if (size != 0) thisStyle.SetSize(new Vector2(size, size));
            if(slice != 0) SetSliceSize(thisStyle, slice);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class Ruleta2D : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField]
    private float sensitivity = 1f;

    [SerializeField]
    private bool invertir = false;

    [SerializeField]
    private float ruletaMin = 10f;

    [SerializeField]
    private float ruletaMax = 200f;

    [SerializeField]
    private float distanciaMinimaCentroPx = 20f;

    private bool estaDragging = false;
    private RectTransform rectTransformRuleta;
    private Camera uiCamera;
    private float zBase;
    private float zRelativoActual;
    private float desfaseArrastre;
    private float anguloMouseAnterior;
    private float anguloMouseContinuo;
    private float xBloqueada;
    private float yBloqueada;

    private Mouse mouse;

    void Start()
    {
        rectTransformRuleta = GetComponent<RectTransform>();
        mouse = Mouse.current;

        Canvas canvasRaiz = GetComponentInParent<Canvas>();
        uiCamera = (canvasRaiz != null) ? canvasRaiz.worldCamera : null;

        Vector3 rotacionInicial = rectTransformRuleta.localEulerAngles;
        zBase = NormalizarAngulo(rotacionInicial.z);
        xBloqueada = rotacionInicial.x;
        yBloqueada = rotacionInicial.y;

        AplicarLimitesRotacion();
    }

    void Update()
    {
        if (mouse == null)
        {
            return;
        }

        // Detectar pulsación del botón izquierdo
        if (mouse.leftButton.wasPressedThisFrame)
        {
            IniciarDrag();
        }

        if (mouse.leftButton.isPressed && estaDragging)
        {
            ActualizarRotacion();
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            estaDragging = false;
        }
    }

    private void IniciarDrag()
    {
        Vector2 posRaton = mouse.position.ReadValue();
        if (!TryObtenerAngulo(posRaton, out float anguloActual))
        {
            estaDragging = false;
            return;
        }

        float factorDireccion = ObtenerFactorDireccion();

        anguloMouseAnterior = anguloActual;
        anguloMouseContinuo = anguloActual;

        // Mantiene continuidad al empezar a arrastrar y evita saltos de -180/180.
        desfaseArrastre = zRelativoActual - (anguloMouseContinuo * sensitivity * factorDireccion);
        estaDragging = true;
    }

    private void ActualizarRotacion()
    {
        Vector2 posRaton = mouse.position.ReadValue();
        if (!TryObtenerAngulo(posRaton, out float anguloActual))
        {
            return;
        }

        float deltaMouse = Mathf.DeltaAngle(anguloMouseAnterior, anguloActual);
        anguloMouseContinuo += deltaMouse;
        anguloMouseAnterior = anguloActual;

        float factorDireccion = ObtenerFactorDireccion();

        float limiteMin = Mathf.Min(ruletaMin, ruletaMax);
        float limiteMax = Mathf.Max(ruletaMin, ruletaMax);
        float zObjetivoRelativo =
            (anguloMouseContinuo * sensitivity * factorDireccion) + desfaseArrastre;

        zRelativoActual = Mathf.Clamp(zObjetivoRelativo, limiteMin, limiteMax);
        float nuevoZ = zBase + zRelativoActual;
        rectTransformRuleta.localEulerAngles = new Vector3(xBloqueada, yBloqueada, nuevoZ);
    }

    private void AplicarLimitesRotacion()
    {
        float limiteMin = Mathf.Min(ruletaMin, ruletaMax);
        float limiteMax = Mathf.Max(ruletaMin, ruletaMax);

        float zActualNormalizada = NormalizarAngulo(rectTransformRuleta.localEulerAngles.z);
        zRelativoActual = Mathf.DeltaAngle(zBase, zActualNormalizada);
        zRelativoActual = Mathf.Clamp(zRelativoActual, limiteMin, limiteMax);

        float zFinal = zBase + zRelativoActual;
        rectTransformRuleta.localEulerAngles = new Vector3(xBloqueada, yBloqueada, zFinal);
    }

    /// <summary>
    /// Calcula el ángulo del ratón respecto al centro del RectTransform en pantalla.
    /// </summary>
    private bool TryObtenerAngulo(Vector2 posRaton, out float angulo)
    {
        Vector2 centroEnPantalla = RectTransformUtility.WorldToScreenPoint(
            uiCamera,
            rectTransformRuleta.position
        );
        Vector2 direccion = posRaton - centroEnPantalla;

        if (direccion.sqrMagnitude < (distanciaMinimaCentroPx * distanciaMinimaCentroPx))
        {
            angulo = 0f;
            return false;
        }

        angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        return true;
    }

    private float NormalizarAngulo(float angulo)
    {
        return Mathf.Repeat(angulo + 180f, 360f) - 180f;
    }

    private float ObtenerFactorDireccion()
    {
        // Dirección natural por defecto; activar "invertir" la invierte.
        return invertir ? -1f : 1f;
    }
}

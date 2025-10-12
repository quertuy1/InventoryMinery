using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InputFieldKeyboardAdjuster : MonoBehaviour
{
    [Header("UI Referencias")]
    public RectTransform panelPrincipal; // El panel que contiene tus inputs

    [Header("Configuración")]
    public float tiempoAnimacion = 0.25f;
    public float margenExtra = 50f; // Espacio extra sobre el input

    private Vector2 posicionOriginal;
    private Coroutine animacionActual;
    private float alturaTecladoEstimada;

    void Start()
    {
        if (panelPrincipal == null)
            panelPrincipal = GetComponent<RectTransform>();

        posicionOriginal = panelPrincipal.anchoredPosition;

        // Estimar altura del teclado según la plataforma
#if UNITY_ANDROID
        alturaTecladoEstimada = Screen.height * 0.45f; // Android suele usar 45%
#elif UNITY_IOS
        alturaTecladoEstimada = Screen.height * 0.40f; // iOS suele usar 40%
#else
        alturaTecladoEstimada = Screen.height * 0.4f;
#endif

        // Auto-suscribirse a TODOS los InputFields
        SubscribirInputFields();
    }

    void SubscribirInputFields()
    {
        // CORRECCIÓN: Usar FindObjectsByType en lugar de FindObjectsOfType
        TMP_InputField[] inputs = FindObjectsByType<TMP_InputField>(FindObjectsSortMode.None);

        foreach (var input in inputs)
        {
            // Cuando se selecciona
            input.onSelect.AddListener((string text) => {
                OnInputFieldSelected(input);
            });

            // Cuando se deselecciona
            input.onEndEdit.AddListener((string text) => {
                OnInputFieldDeselected();
            });
        }

        Debug.Log($"✅ Se encontraron {inputs.Length} InputFields y se configuraron para el teclado");
    }

    void OnInputFieldSelected(TMP_InputField input)
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!gameObject.activeInHierarchy) return;
        
        StartCoroutine(AjustarPosicionParaInput(input));
#endif
    }

    void OnInputFieldDeselected()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!gameObject.activeInHierarchy) return;
        
        if (animacionActual != null)
            StopCoroutine(animacionActual);
            
        animacionActual = StartCoroutine(AnimarPanel(posicionOriginal));
#endif
    }

    IEnumerator AjustarPosicionParaInput(TMP_InputField input)
    {
        // Esperar a que el teclado aparezca
        yield return new WaitForSeconds(0.1f);

        // Obtener la posición del input en coordenadas de pantalla
        RectTransform inputRect = input.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        inputRect.GetWorldCorners(corners);

        float posicionInferiorInput = corners[0].y; // Borde inferior del input

        // Si el input está debajo del teclado, mover el panel
        if (posicionInferiorInput < alturaTecladoEstimada)
        {
            float desplazamientoNecesario = alturaTecladoEstimada - posicionInferiorInput + margenExtra;
            Vector2 nuevaPosicion = posicionOriginal + new Vector2(0, desplazamientoNecesario);

            if (animacionActual != null)
                StopCoroutine(animacionActual);

            animacionActual = StartCoroutine(AnimarPanel(nuevaPosicion));
        }
    }

    IEnumerator AnimarPanel(Vector2 targetPos)
    {
        Vector2 startPos = panelPrincipal.anchoredPosition;
        float elapsed = 0;

        while (elapsed < tiempoAnimacion)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / tiempoAnimacion;
            // Curva de animación suave
            t = t * t * (3f - 2f * t);

            panelPrincipal.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        panelPrincipal.anchoredPosition = targetPos;
    }

    // Para cuando se agregan inputs dinámicamente
    public void RefrescarInputFields()
    {
        SubscribirInputFields();
    }
}
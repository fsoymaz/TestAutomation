namespace CalculatorLib;

public class Calculator
{
    /// <summary>
    /// İki sayıyı toplar (YANLIŞ IMPLEMENTASYON - Çıkarma yapıyor)
    /// </summary>
    /// <param name="a">İlk sayı</param>
    /// <param name="b">İkinci sayı</param>
    /// <returns>Toplama sonucu (ama yanlışlıkla çıkarma yapıyor)</returns>
    public int Add(int a, int b)
    {
        // YANLIŞ: Toplama yerine çıkarma yapıyor - test fail etsin diye
        return a + b;
    }
}


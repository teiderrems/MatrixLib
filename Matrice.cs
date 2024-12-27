namespace MatriceLib;

public class Matrice
{
    private int[] 
        Shape { get;}
    private double[][] Matrix { get;}


    public override int GetHashCode()
    {
        return Shape[0]*Shape[1];
    }

    /**
     *  fonction pour vérifier l'égalité de deux matrices
     *
     */
    public override bool Equals(object? obj)
    {
        if (obj is Matrice matrice)
        {
            if (matrice.Shape[0]==this.Shape[0] && matrice.Shape[1]==this.Shape[1])
            {
                for (var i = 0; i < matrice.Shape[0]; i++)
                {
                    for (var j = 0; j < matrice.Shape[1]; j++)
                    {
                        if (Math.Abs(matrice.Matrix[i][j] - this.Matrix[i][j])!=0)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        return false;
    }

    /**
     *  fonction pour afficher les coéfficients de la matricielle sous forme de chaine de caractère
     *
     */
    public override string ToString()
    {
        string result = "\n";
        for (var i = 0; i < Shape[0]; i++)
        {
            for (var j = 0; j < Shape[1]; j++)
            {
                result += $"\t{Matrix[i][j]}";
            }
            result += "\n";
        }
        return $"{result}";
    }


    /**
     *  opérateur pour facilité l'addition matricielle
     *
     */
    public static Matrice operator+(Matrice matrice1,Matrice matrice2)
    {
        if ((matrice1.Shape[0], matrice1.Shape[1])!=(matrice2.Shape[0], matrice2.Shape[1])){
            throw new Exception("Matrice must have the same size");
        }
        double[][] tmp = new double[matrice2.
                Shape[0]][];
        
        for (var i = 0; i < matrice1.
                Shape[0]; i++)
        {
            tmp[i] = new double[matrice1.
                    Shape[1]];
            for (var j = 0; j < matrice1.
                    Shape[1]; j++)
            {
                tmp[i][j]=matrice2.Matrix[i][j]+matrice1.Matrix[i][j];
            }
        }
        return new Matrice(tmp);
    }
    
    /**
     *  opérateur pour facilité la soustraction matricielle
     *
     */
    public static Matrice operator-(Matrice matrice1,Matrice matrice2)
    {
        if ((matrice1.Shape[0], matrice1.Shape[1])!=(matrice2.Shape[0], matrice2.Shape[1])){
            throw new Exception("Matrice must have the same size");
        }
        double[][] tmp = new double[matrice2.
                Shape[0]][];
        
        for (var i = 0; i < matrice1.
                Shape[0]; i++)
        {
            tmp[i] = new double[matrice1.
                    Shape[1]];
            for (var j = 0; j < matrice1.
                    Shape[1]; j++)
            {
                tmp[i][j]=matrice2.Matrix[i][j]-matrice1.Matrix[i][j];
            }
        }
        return new Matrice(tmp);
    }
    
    /**
     *  opérateur pour facilité le produit d'Hadamard
     *
     */
    public static Matrice operator*(Matrice matrice1,Matrice matrice2)
    {
        if ((matrice1.Shape[0], matrice1.Shape[1])!=(matrice2.Shape[0], matrice2.Shape[1])){
            throw new Exception("Matrice must have the same size");
        }
        double[][] tmp = new double[matrice2.
                Shape[0]][];
        
        for (var i = 0; i < matrice1.
                Shape[0]; i++)
        {
            tmp[i] = new double[matrice1.
                    Shape[1]];
            for (var j = 0; j < matrice1.
                    Shape[1]; j++)
            {
                tmp[i][j]=matrice2.Matrix[i][j]*matrice1.Matrix[i][j];
            }
        }
        return new Matrice(tmp);
    }
    
    /**
     *  opérateur pour facilité le produit d'Hadamard
     *
     */
    public static Matrice operator/(Matrice matrice1,Matrice matrice2)
    {
        if ((matrice1.Shape[0], matrice1.Shape[1])!=(matrice2.Shape[0], matrice2.Shape[1])){
            throw new Exception("Matrice must have the same size");
        }
        double[][] tmp = new double[matrice2.
                Shape[0]][];
        
        for (var i = 0; i < matrice1.
                Shape[0]; i++)
        {
            tmp[i] = new double[matrice1.
                    Shape[1]];
            for (var j = 0; j < matrice1.
                    Shape[1]; j++)
            {
                tmp[i][j]=matrice2.Matrix[i][j]/matrice1.Matrix[i][j];
            }
        }
        return new Matrice(tmp);
    }

    /**
     *  opérateur pour facilité la multiplication des coéfficients d'une matrice avec une constante
     *
     */
    public static Matrice operator*(Matrice matrice,double cte)
    {
        double[][] tmp = matrice.Matrix;
        for (var i = 0; i < matrice.Shape[0]; i++)
        {
            for (var j = 0; j < matrice.Shape[1]; j++)
            {
                tmp[i][j]*=cte;
            }
        }

        return new Matrice(tmp);
    }



    /**
     *  opérateur pour facilité le produit matrice et vecteur
     *
     */
    public static double[] operator *(Matrice matrice, double[] array)
    {
        if (array.Length == matrice.Shape[1])
        {
            double[] result = new double[matrice.Shape[0]];
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                double som = 0.0;
                for (int j = 0; j < matrice.Shape[1]; j++)
                {
                    som += matrice.Matrix[i][j] * array[j];
                }
                result[i] = som;
            }
            return result;
        }
        else
        {
            throw new Exception("incompatible size");
        }
    }

    /**
     *  opérateur pour facilité l'addition des coéfficients d'une matrice avec une constante
     *  
     */
    public static Matrice operator+(Matrice matrice,double cte)
    {
        double[][] tmp = matrice.Matrix;
        for (var i = 0; i < matrice.Shape[0]; i++)
        {
            for (var j = 0; j < matrice.Shape[1]; j++)
            {
                tmp[i][j]+=cte;
            }
        }

        return new Matrice(tmp);
    }
    
    /**
     *  opérateur pour facilité la soustration des coéfficients d'une matrice avec une constante
     *
     */
    public static Matrice operator-(Matrice matrice,double cte)
    {
        double[][] tmp = matrice.Matrix;
        for (var i = 0; i < matrice.Shape[0]; i++)
        {
            for (var j = 0; j < matrice.Shape[1]; j++)
            {
                tmp[i][j]-=cte;
            }
        }

        return new Matrice(tmp);
    }
    
    /**
     *  opérateur pour facilité la soustration des coéfficients d'une matrice avec une constante
     *
     */
    public static Matrice operator/(Matrice matrice,double cte)
    {
        double[][] tmp = matrice.Matrix;
        for (var i = 0; i < matrice.Shape[0]; i++)
        {
            for (var j = 0; j < matrice.Shape[1]; j++)
            {
                tmp[i][j]/=cte;
            }
        }

        return new Matrice(tmp);
    }

    
    /**
     *  fonction pour vérifier l'égalité de deux matrices
     *
     */
    public static bool operator ==(Matrice matrice1, Matrice matrice2)
    {
        if (matrice1.Shape[0]==matrice2.Shape[0] && matrice1.Shape[1]==matrice2.Shape[1])
        {
            for (var i = 0; i < matrice1.Shape[0]; i++)
            {
                for (var j = 0; j < matrice1.Shape[1]; j++)
                {
                    if (Math.Abs(matrice1.Matrix[i][j] - matrice2.Matrix[i][j])!=0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        return false;
    }
    
    /**
     *  fonction pour vérifier si deux matrices ont aumoins un coéfficient différent
     *
     */
    public static bool operator !=(Matrice matrice1, Matrice matrice2)
    {
        if (matrice1.Shape[0]==matrice2.Shape[0] && matrice1.Shape[1]==matrice2.Shape[1])
        {
            for (var i = 0; i < matrice1.Shape[0]; i++)
            {
                for (var j = 0; j < matrice1.Shape[1]; j++)
                {
                    if (Math.Abs(matrice1.Matrix[i][j] - matrice2.Matrix[i][j])!=0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    
    /**
     * Fonction pour faire le produit matricielle
     */
    public  Matrice Dot(Matrice matrice1)
    {
        
        if (matrice1.Shape[1]!=this.Shape[0])
        {
            throw new Exception("colonne number of matrice1 should be the same than row number of current matrice ");
        }
        
        double[][] tmp = new double[this.Shape[0]][];
        for (var i = 0; i < matrice1.
                 Shape[0]; i++)
        {
            tmp[i] = new double[this.
                Shape[1]];
            double som = 0;
            for (var j = 0; j < this.
                     Shape[1]; j++)
            {
                for (var k = 0; k < this.Shape[0]; k++)
                {
                    som+=matrice1.Matrix[i][k]*this.Matrix[k][j];
                }
                tmp[i][j] = som;
            }
        }
        return new Matrice(tmp);
    }
    
    public static double[] Dot(Matrice matrice, double[] array)
    {
        if (array.Length == matrice.Shape[1])
        {
            double[] result = new double[matrice.Shape[0]];
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                double som = 0.0;
                for (int j = 0; j < matrice.Shape[1]; j++)
                {
                    som += matrice.Matrix[i][j] * array[j];
                }
                result[i] = som;
            }
            return result;
        }
        else
        {
            throw new Exception("incompatible size");
        }
    }

    /**
     * Fonction pour calculer la transposée d'une matrice
     */
    public Matrice Transpose()
    {
        
        double[][] matrix = new double[Shape[1]][];
        for (var i = 0; i < Shape[1]; i++)
        {
            matrix[i] = new double[Shape[0]];
            for (var j = 0; j < matrix[i].Length; j++)
            {
                
                matrix[i][j] = Matrix[j][i];
                
            }
        }
        return new Matrice(matrix);
    }

    public Matrice(double[][] matrix)
    {
        Matrix = matrix;
        
        Shape=[matrix.Length,matrix[0].Length];
    }
    
    public Matrice(List<List<double>> matrix)
    {
        Matrix = new double[matrix.Count][];
        for (var i = 0; i < matrix.Count; i++)
        {
            Matrix[i] = matrix[i].ToArray();
        }
        Shape=[matrix.Count,matrix[0].Count];
    }

    public Matrice(int[] shape)
    {
        
        Shape = shape;
        Matrix = new double[
                Shape[0]][];
        for (var i = 0; i < 
                Shape[0]; i++)
        {
            Matrix[i] = new double[
                    Shape[1]];
        }
    }

    public Matrice(double[] array,int[] shape)
    {
        Shape=shape;
        Matrix=new double[
                Shape[0]][];
        var k = 0;
        for (var i = 0; i < 
                Shape[0]; i++)
        {
            Matrix[i] = new double[
                    Shape[0]];
            for (var j = 0; j < 
                    Shape[1]; j++)
            {
                Matrix[i][j] = array[k];
                k++;
            }
        }
    }
    
    public Matrice(List<double> array,int[] shape)
    {
        Shape=shape;
        Matrix=new double[
                Shape[0]][];
        var k = 0;
        for (var i = 0; i < 
                Shape[0]; i++)
        {
            Matrix[i] = new double[
                    Shape[0]];
            for (var j = 0; j < 
                    Shape[1]; j++)
            {
                Matrix[i][j] = array[k];
                k++;
            }
        }
    }

    public  static double Determinant(double[][] matrice, int j = 0)
    {

        if (matrice.Length == 2 && matrice[0].Length == 2)
        {
            return matrice[0][0] * matrice[1][1] - matrice[1][0] * matrice[0][1];
        }
        else
        {
            double deter = 0.0;
            for (int i = j; i < matrice[0].Length; i++)
            {
                deter += Math.Pow(-1.0, (double)i) * matrice[0][i] * Determinant(ExtractCoMatrix(matrice, i), i);
            }
            return deter;
        }
    }

    public Matrice Copy() => new(Matrix);

    public Matrice Adjacence()
    {
        Matrice mat = new(Shape);
        for (int i = 0; i < mat.Shape[0]; i++)
        {
            for (int j = 0; j < mat.Shape[1]; j++)
            {
                mat.Matrix[i][j] = Math.Pow(-1.0,(double)(i+j))*Determinant(ExtractCoFacteur(Copy().Transpose().Matrix, i, j));
            }
        }
        return mat;
    }


    public Matrice Hstack(double[] array, string place = "front")
    {
        Matrice matrice = new([Shape[0], Shape[1] + 1]);
        if (place=="end")
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                matrice.Matrix[i][Shape[1]] = array[i];
                for (int j = 0; j < Shape[1]; j++)
                {
                    matrice.Matrix[i][j] = Matrix[i][j];
                }
            }
        }
        else
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                matrice.Matrix[i][0] = array[i];
                for (int j = 0; j < Shape[1]; j++)
                {
                    matrice.Matrix[i][j + 1] = Matrix[i][j];
                }
            }
        }
        return matrice;
    }

    public Matrice Hstack(double[][] array, string place = "front")
    {
        Matrice mat = new(array);
        Matrice matrice = new([Shape[0], Shape[1] + mat.Shape[0]]);
        mat = mat.Transpose();

        if (place=="end")
        {
            for (int i = 0; i < Shape[0]; i++)
            {
                for (int j = 0; j < Shape[1]; j++)
                {
                    matrice.Matrix[i][j] = Matrix[i][j];
                }
            }
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                for (int j = Shape[1]; j < matrice.Shape[1]; j++)
                {
                    matrice.Matrix[i][j] = mat.Matrix[i][j - Shape[1]];
                }
            }
        }
        else
        {
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                for (int j = 0; j < mat.Shape[1]; j++)
                {
                    matrice.Matrix[i][j] = mat.Matrix[i][j];
                }
            }
            for (int i = 0; i < matrice.Shape[0]; i++)
            {
                for (int j = 0; j < Shape[1]; j++)
                {
                    matrice.Matrix[i][j + mat.Shape[1]] = Matrix[i][j];
                }
            }
        }
        return matrice;
    }

    public Matrice Vstack(double[] array,string place="front")
    {
        Matrice matrice = new([Shape[0] + 1, Shape[1]]);
        switch (place)
        {
            case "front":
                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    if (i == 0)
                    {
                        matrice.Matrix[i] = array;
                    }
                    else
                    {
                        matrice.Matrix[i] = Matrix[i - 1];
                    }
                }
                break;
            case "end":
                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    if (i == matrice.Shape[0]-1)
                    {
                        matrice.Matrix[i] = array;
                    }
                    else
                    {
                        matrice.Matrix[i] = Matrix[i];
                    }
                }
                break;
            default:
                for (int i = 0; i < matrice.Shape[0]; i++)
                {
                    if (i == 0)
                    {
                        matrice.Matrix[i] = array;
                    }
                    else
                    {
                        matrice.Matrix[i] = Matrix[i - 1];
                    }
                }
                break;
        }
        return matrice;
    }



    public Matrice Vstack(double[][] array, string place = "front")
    {
        Matrice matrice = new([Shape[0] + array.Length, Shape[1]]);
        switch (place)
        {
            case "front":
                for (int i = 0; i < array.Length; i++)
                {
                    matrice.Matrix[i] = array[i];
                }
                for (int i = array.Length; i < matrice.Shape[0]; i++)
                {
                   matrice.Matrix[i] = Matrix[i-array.Length];
                }
                break;
            case "end":
                for (int i = 0; i < Shape[0]; i++)
                {
                   matrice.Matrix[i] = Matrix[i];
                }
                for (int i = Shape[0]; i < matrice.Shape[0]; i++)
                {
                    matrice.Matrix[i] = array[i - Shape[0]];
                }
                break;
            default:
                for (int i = 0; i < array.Length; i++)
                {
                    matrice.Matrix[i] = array[i];
                }
                for (int i = array.Length; i < matrice.Shape[0]; i++)
                {
                   matrice.Matrix[i] = Matrix[i-array.Length];
                }
                break;
        }
        return matrice;
    }

    public static double[][] ExtractCoFacteur(double[][] matrice, int i, int j)
    {

        int m = matrice.Length - 1;
        double[][] result = new double[m][];

        int k = 0;
        int t = 0;

        while (k < matrice.Length)
        {
            if (k != i)
            {
                double[] tmp = new double[m];
                int z = 0;
                int e = 0;
                while (e < matrice[k].Length)
                {
                    if (e != j)
                    {
                        tmp[z++] = matrice[k][e];
                    }
                    e++;
                }
                result[t++] = tmp;
            }
            k++;
        }
        return result;
    }

    private static double[][] ExtractCoMatrix(double[][] matrice,int j)
    {

        int m = matrice.Length-1;
        double[][] result = new double[m][];

        int k = 1;
        int t = 0;

        while (k < matrice.Length)
        {
            double[] tmp = new double[m];
            int z = 0;
            int e = 0;
            while (e < matrice[k].Length)
            {
                if (e != j)
                {
                    tmp[z++]=matrice[k][e];
                }
                e++;
            }
            result[t++]=tmp;
            k++;
        }
        return result;
    }


    public double Sum()
    {
        double som = 0.0;
        for (int i = 0; i < Shape[0]; i++)
        {
            for (int j = 0; j < Shape[1]; j++)
            {
                som += Matrix[i][j];
            }
        }
        return som;
    }

    public double[] CumSum()
    {
        double[] som = new double[Shape[0]];
        for (int i = 0; i < Shape[0]; i++)
        {
            for (int j = 0; j < Shape[1]; j++)
            {
                som[i] += Matrix[i][j];
            }
        }
        return som;
    }

    public double Mean()
    {
        return Sum() / (Shape[0] * Shape[1]);
    }

    public double[] CumMean()
    {
        double[] arrayMean = new double[Shape[0]];
        double[] arraySum = CumSum();
        for (int i = 0; i < Shape[0]; i++)
        {
            arrayMean[i] = arraySum[i] / Shape[0];
        }
        return arrayMean;
    }

    public double Var()
    {
        double mean = Mean();
        double cumsum = 0.0;
        for (int i = 0; i < Shape[0]; i++)
        {
            for (int j = 0; j < Shape[1]; j++)
            {
                cumsum += Math.Pow(Matrix[i][j] - mean, 2.0);
            }
        }
        return cumsum / (Shape[0] * Shape[1]);
    }

    public double Sd()
    {
        return Math.Sqrt(Var());
    }

    public double[] CumVar()
    {
        double[] arrayVar = new double[Shape[0]];
        double[] arrayMean = CumMean();
        for (int i = 0; i < Shape[0]; i++)
        {
            for(int j = 0; j < Shape[1]; j++)
            {
                arrayVar[i] += (Math.Pow(Matrix[i][j] - arrayMean[i], 2.0)/Shape[1]);
            }
        }
        return arrayVar;
    }

    public double[] CumSd()
    {
        double[] cumVar = CumVar();
        double[] cumsd = new double[Shape[0]];
        for (int i = 0; i < cumVar.Length; i++)
        {
            cumsd[i] = Math.Sqrt(cumVar[i]);
        }
        return cumsd;
    }

    public Matrice Inverse()
    {
        double determinant = Determinant(Matrix);
        if (determinant==0)
        {
            throw new Exception("non-invertible matrix because its determinant is zero");
        }
        else
        {
            Matrice matrice = Adjacence();
            matrice /= determinant;
            return matrice;
        }
    }

    public double[] Solve(double[] beta) => Inverse() * beta;
}
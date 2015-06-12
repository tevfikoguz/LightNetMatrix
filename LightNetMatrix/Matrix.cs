﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace LightNetMatrix
{
    /// <summary>
    /// Represents a two dimensional dense matrix of real (double precision) elements.
    /// </summary>
    [DebuggerDisplay("Matrix {RowCount} x {ColumnCount}")]
    //[Serializable]
    [CLSCompliant(true)]
    public class Matrix
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class with specified dimensions.
        /// </summary>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        public Matrix(int rowCount, int columnCount)
        {
            this.rowCount = rowCount;
            this.columnCount = columnCount;

            this.coreArray = new double[rowCount*columnCount];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class of specified dimension.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        public Matrix(int dimension) : this(dimension, dimension)
        {
        }


        public double[] coreArray;

        /// <summary>
        /// Gets the number of rows of the matrix.
        /// </summary>
        
        public int RowCount
        {
            get { return rowCount; }
        }

        /// <summary>
        /// Gets the number of columns of the matrix.
        /// </summary>
        public int ColumnCount
        {
            get { return columnCount; }
        }

        /// <summary>
        /// Number of rows of the matrix.
        /// </summary>
        [XmlElement(ElementName = "RowCount")]
        private int rowCount;

        /// <summary>
        /// Number of columns of the matrix.
        /// </summary>
        [XmlElement(ElementName = "ColumnCount")]
        private int columnCount;

        /// <summary>
        /// Gets or sets the specified member.
        /// </summary>
        /// <value>
        /// The <see cref="System.Double"/>.
        /// </value>
        /// <param name="row">The row (zero based).</param>
        /// <param name="column">The column (zero based).</param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.IndexerName("TheMember")]
        public double this[int row, int column]
        {
            get
            {
                if (row >= this.RowCount || column >= this.ColumnCount)
                    throw new Exception("Invalid column or row specified");

                return this.coreArray[column*this.rowCount + row];
            }

            set
            {
                if (row >= this.RowCount || column >= this.ColumnCount)
                    throw new Exception("Invalid column or row specified");

                this.coreArray[column*this.rowCount + row] = value;
            }
        }

        /// <summary>
        /// Multiplies the specified matrices.
        /// </summary>
        /// <param name="m1">The m1.</param>
        /// <param name="m2">The m2.</param>
        /// <returns>m1 * m2</returns>
        public static Matrix Multiply(Matrix m1, Matrix m2)
        {
            if (m1.ColumnCount != m2.RowCount)
                throw new InvalidOperationException("No consistent dimensions");

            var res = new Matrix(m1.RowCount, m2.ColumnCount);

            for (int i = 0; i < m1.rowCount; i++)
                for (int j = 0; j < m2.columnCount; j++)
                    for (int k = 0; k < m1.columnCount; k++)
                    {
                        res.coreArray[j*res.rowCount + i] +=
                            m1.coreArray[k*m1.rowCount + i]*
                            m2.coreArray[j*m2.rowCount + k];
                    }


            return res;
        }

        /// <summary>
        /// Throws an exception with specified <see cref="message"/> if <see cref="condition"/> is true.
        /// </summary>
        /// <param name="condition">the condition.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.Exception">If conditions met!</exception>
        private static void ThrowIf(bool condition, string message)
        {
            if (condition)
                throw new Exception(message);
        }

        /// <summary>
        /// To2s the d double array.
        /// </summary>
        /// <param name="mtx">The MTX.</param>
        /// <returns></returns>
        public static double[,] To2DDoubleArray(Matrix mtx)
        {
            var buf = new double[mtx.RowCount, mtx.ColumnCount];

            for (int i = 0; i < mtx.RowCount; i++)
                for (int j = 0; j < mtx.ColumnCount; j++)
                    buf[i, j] = mtx.coreArray[j*mtx.RowCount + i];

            return buf;
        }

        /// <summary>
        /// Creates a new Identity matrix.
        /// </summary>
        /// <param name="n">The matrix dimension.</param>
        /// <returns>Eye matrix of size <see cref="n"/></returns>
        public static Matrix Eye(int n)
        {
            var buf = new Matrix(n, n);

            for (int i = 0; i < n; i++)
                buf.coreArray[i*n + i] = 1.0;

            return buf;
        }

        /// <summary>
        /// Creates <see cref="m"/> by <see cref="n"/> matrix filled with zeros.
        /// </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <returns><see cref="m"/>x<see cref="n"/> matrix filled with zeros.</returns>
        public static Matrix Zeros(int m, int n)
        {
            return new Matrix(m, n);
        }

        /// <summary>
        /// Creates n by n matrix filled with zeros.
        /// </summary>       
        /// <param name="n">Number of rows and columns, resp.</param>
        /// <returns>n x n matrix filled with zeros.</returns>
        public static Matrix Zeros(int n)
        {
            return new Matrix(n);
        }

        /// <summary>
        /// Creates row by n matrix filled with ones.
        /// </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <returns>m x n matrix filled with zeros.</returns>
        public static Matrix Ones(int m, int n)
        {
            var buf = new Matrix(m, n);

            for (int i = 0; i < m*n; i++)
                buf.coreArray[i] = 1.0;

            return buf;
        }

        /// <summary>
        /// Creates n by n matrix filled with ones.
        /// </summary>        
        /// <param name="n">Number of columns.</param>
        /// <returns>n x n matrix filled with ones.</returns>        
        public static Matrix Ones(int n)
        {
            var buf = new Matrix(n);

            for (int i = 0; i < n*n; i++)
                buf.coreArray[i] = 1.0;

            return buf;
        }

        /// <summary>
        /// Computes product of main diagonal entries.
        /// </summary>
        /// <returns>Product of diagonal elements</returns>
        public double DiagProd()
        {
            var buf = 1.0;
            int dim = System.Math.Min(this.rowCount, this.columnCount);

            for (int i = 0; i < dim; i++)
            {
                buf *= this.coreArray[i*this.RowCount + i];
            }

            return buf;
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="m1">The m1.</param>
        /// <param name="m2">The m2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            return Matrix.Multiply(m1, m2);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="coeff">The coeff.</param>
        /// <param name="mat">The mat.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix operator *(double coeff, Matrix mat)
        {
            var newMat = new double[mat.RowCount*mat.ColumnCount];


            for (int i = 0; i < newMat.Length; i++)
            {
                newMat[i] = coeff*mat.coreArray[i];
            }

            var buf = new Matrix(mat.RowCount, mat.ColumnCount);
            buf.coreArray = newMat;

            return buf;
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="mat">The mat.</param>
        /// <param name="coeff">The coeff.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix operator *(Matrix mat, double coeff)
        {
            var newMat = new double[mat.RowCount*mat.ColumnCount];


            for (int i = 0; i < newMat.Length; i++)
            {
                newMat[i] = coeff*mat.coreArray[i];
            }

            var buf = new Matrix(mat.RowCount, mat.ColumnCount);
            buf.coreArray = newMat;

            return buf;
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="mat">The mat.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix operator -(Matrix mat)
        {
            var buf = new Matrix(mat.RowCount, mat.ColumnCount);
            ;

            for (int i = 0; i < buf.coreArray.Length; i++)
            {
                buf.coreArray[i] = -mat.coreArray[i];
            }

            return buf;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="mat1">The mat1.</param>
        /// <param name="mat2">The mat2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix operator +(Matrix mat1, Matrix mat2)
        {
            ThrowIf(mat1.RowCount != mat2.RowCount || mat1.ColumnCount != mat2.ColumnCount,
                "Inconsistent matrix sizes");

            var buf = new Matrix(mat1.RowCount, mat1.ColumnCount);

            for (int i = 0; i < buf.coreArray.Length; i++)
            {
                buf.coreArray[i] = mat1.coreArray[i] + mat2.coreArray[i];
            }

            return buf;
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="mat1">The mat1.</param>
        /// <param name="mat2">The mat2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix operator -(Matrix mat1, Matrix mat2)
        {
            ThrowIf(mat1.RowCount != mat2.RowCount || mat1.ColumnCount != mat2.ColumnCount,
                "Inconsistent matrix sizes");

            var buf = new Matrix(mat1.RowCount, mat1.ColumnCount);

            for (int i = 0; i < buf.coreArray.Length; i++)
            {
                buf.coreArray[i] = mat1.coreArray[i] - mat2.coreArray[i];
            }

            return buf;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Matrix left, Matrix right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Matrix left, Matrix right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Sets the specified row of matrix with defined values.
        /// </summary>
        /// <param name="i">The row number.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">values</exception>
        public void SetRow(int i, params double[] values)
        {
            if (values.Length != this.ColumnCount)
                throw new ArgumentOutOfRangeException("values");

            for (int j = 0; j < this.ColumnCount; j++)
            {
                this.coreArray[j * this.RowCount + i] = values[j];
            }
        }

        /// <summary>
        /// Sets the specified column of matrix with defined values.
        /// </summary>
        /// <param name="j">The column number.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">values</exception>
        public void SetColumn(int j, params double[] values)
        {
            if (values.Length != this.RowCount)
                throw new ArgumentOutOfRangeException("values");


            for (int i = 0; i < this.RowCount; i++)
            {
                this.coreArray[j * this.RowCount + i] = values[i];
            }
        }

        /// <summary>
        /// Provides a shallow copy of this matrix.
        /// </summary>
        /// <returns>Clone of this matrix</returns>
        public Matrix Clone()
        {
            var buf = new Matrix(this.RowCount, this.ColumnCount);

            buf.coreArray = (double[])this.coreArray.Clone();
            return buf;
        }

        /// <summary>
        /// Swaps each matrix entry A[i, j] with A[j, i].
        /// </summary>
        /// <returns>Transposed of this matrix.</returns>
        public Matrix Transpose()
        {
            var buf = new Matrix(this.ColumnCount, this.RowCount);

            var newMatrix = buf.coreArray;

            for (int row = 0; row < this.RowCount; row++)
                for (int column = 0; column < this.ColumnCount; column++)
                    //newMatrix[column*this.RowCount + row] = this.coreArray[row*this.RowCount + column];
                    buf[column, row] = this[row, column];

            buf.coreArray = newMatrix;
            return buf;
        }

        protected bool Equals(Matrix other)
        {
            if (other.RowCount != this.RowCount)
                return false;

            if (other.ColumnCount != this.ColumnCount)
                return false;

            for (int i = 0; i < other.coreArray.Length; i++)
            {
                if (!FuzzyEquals(this.coreArray[i], other.coreArray[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Matrix)obj);
        }

        /// <summary>
        /// Checks if number of rows equals number of columns.
        /// </summary>
        /// <returns>True iff matrix is n by n.</returns>
        public bool IsSquare()
        {
            return (this.columnCount == this.rowCount);
        }

        public bool IsSymmetric()
        {
            for (int i = 0; i < this.rowCount; i++)
                for (int j = i + 1; j < this.columnCount; j++)
                    if (!FuzzyEquals(this[i, j], this[j, i]))
                    {
                        return false;
                    }

            return true;
        }

        /// <summary>
        /// Checks if A[i, j] == 0 for i < j.
        /// </summary>
        /// <returns>True iff matrix is upper trapeze.</returns>
        public bool IsUpperTrapeze()
        {
            for (int j = 1; j <= columnCount; j++)
                for (int i = j + 1; i <= rowCount; i++)
                    if (!FuzzyEquals(this.coreArray[j * this.RowCount + i], 0))
                        return false;

            return true;
        }

        /// <summary>
        /// Checks if A[i, j] == 0 for i > j.
        /// </summary>
        /// <returns>True iff matrix is lower trapeze.</returns>
        public bool IsLowerTrapeze()
        {
            for (int i = 1; i <= rowCount; i++)
                for (int j = i + 1; j <= columnCount; j++)
                    if (!FuzzyEquals(this.coreArray[j*this.RowCount + i], 0.0))
                        return false;

            return true;
        }

        /// <summary>
        /// Checks if matrix is lower or upper trapeze.
        /// </summary>
        /// <returns>True iff matrix is trapeze.</returns>
        public bool IsTrapeze()
        {
            return (this.IsUpperTrapeze() || this.IsLowerTrapeze());
        }

        /// <summary>
        /// Checks if matrix is trapeze and square.
        /// </summary>
        /// <returns>True iff matrix is triangular.</returns>
        public bool IsTriangular()
        {
            return (this.IsLowerTriangular() || this.IsUpperTriangular());
        }

        /// <summary>
        /// Checks if matrix is square and upper trapeze.
        /// </summary>
        /// <returns>True iff matrix is upper triangular.</returns>
        public bool IsUpperTriangular()
        {
            return (this.IsSquare() && this.IsUpperTrapeze());
        }

        /// <summary>
        /// Checks if matrix is square and lower trapeze.
        /// </summary>
        /// <returns>True iff matrix is lower triangular.</returns>
        public bool IsLowerTriangular()
        {
            return (this.IsSquare() && this.IsLowerTrapeze());
        }

        /// <summary>
        /// Swaps rows at specified indices. The latter do not have to be ordered.
        /// When equal, nothing is done.
        /// </summary>
        /// <param name="i1">One-based index of first row.</param>
        /// <param name="i2">One-based index of second row.</param>        
        public void SwapRows(int i1, int i2)
        {
            if (i1 < 0 || i1 >= rowCount || i2 < 0 || i2 >= rowCount)
                throw new ArgumentException("Indices must be positive and <= number of rows.");

            if (i1 == i2)
                return;

            for (int i = 0; i < columnCount; i++)
            {
                var tmp = this[i1, i];

                this[i1, i] = this[i2, i];
                this[i2, i] = tmp;
            }
        }

        /// <summary>
        /// Swaps columns at specified indices. The latter do not have to be ordered.
        /// When equal, nothing is done.
        /// </summary>
        /// <param name="j1">One-based index of first col.</param>
        /// <param name="j2">One-based index of second col.</param>       
        public void SwapColumns(int j1, int j2)
        {
            if (j1 <= 0 || j1 > columnCount || j2 <= 0 || j2 > columnCount)
                throw new ArgumentException("Indices must be positive and <= number of cols.");

            if (j1 == j2)
                return;

            var j1Col = this.ExtractColumn(j1).coreArray;
            var j2Col = this.ExtractColumn(j2).coreArray;

            this.SetRow(j1, j2Col);
            this.SetRow(j2, j1Col);
        }

        /// <summary>
        /// Retrieves row vector at specfifed index and deletes it from matrix.
        /// </summary>
        /// <param name="i">One-based index at which to extract.</param>
        /// <returns>Row vector.</returns>
        public Matrix ExtractRow(int i)
        {
            if (i >= this.RowCount || i < 0)
                throw new ArgumentOutOfRangeException("i");

            var mtx = new Matrix(1, this.ColumnCount);


            for (int j = 0; j < this.ColumnCount; j++)
            {
                mtx.coreArray[j] = this.coreArray[j * this.RowCount + i];
            }

            return mtx;
        }

        /// <summary>
        /// Retrieves column vector at specfifed index and deletes it from matrix.
        /// </summary>
        /// <param name="j">One-based index at which to extract.</param>
        /// <returns>Row vector.</returns>
        public Matrix ExtractColumn(int j)
        {
            if (j >= this.ColumnCount || j < 0)
                throw new ArgumentOutOfRangeException("j");

            var mtx = new Matrix(this.RowCount, 1);


            for (int i = 0; i < this.RowCount; i++)
            {
                mtx.coreArray[i] = this.coreArray[j * this.RowCount + i];
            }

            return mtx;
        }

        /// <summary>
        /// Gets the determinant of matrix
        /// </summary>
        /// <returns></returns>
        public double Determinant()
        {
            // Seems working good!

            if (!IsSquare())
                throw new InvalidOperationException();

            var clone = this.Clone();

            var n = this.rowCount;

            var sign = 1.0;

            var epsi1on = 1e-10 * MinAbs(clone.coreArray);

            if (epsi1on == 0)
                epsi1on = 1e-9;

            // this[row,column] = this.coreArray[column*this.rowCount + row]

            for (var i = 0; i < n - 1; i++)
            {
                if (System.Math.Abs(clone[i, i]) < epsi1on)
                {
                    var firstNonZero = -1;

                    for (var k = i + 1; k < n; k++)
                        if (System.Math.Abs(clone[k, i]) > epsi1on)
                            firstNonZero = k;

                    if (firstNonZero == -1)
                        throw new OperationCanceledException();
                    else
                    {
                        clone.SwapRows(firstNonZero, i);
                        sign = -sign;
                    }
                }



                for (var j = i + 1; j < n; j++)
                {
                    var alfa = (clone.coreArray[j * n + i] / clone.coreArray[i * n + i]);

                    for (var k = i; k < n; k++)
                    {
                        clone.coreArray[j * n + k] -= alfa * clone.coreArray[i * n + k];
                    }
                }
            }

            var buf = sign;

            var arr = new double[n];

            for (var i = 0; i < n; i++)
                arr[i] = clone.coreArray[i * n + i];

            Array.Sort(arr);

            for (var i = 0; i < n; i++)
                buf = buf * arr[n - i - 1];

            return buf;
        }

        private static double MinAbs(double[] arr)
        {
            var min = double.MaxValue;

            foreach (var d in arr)
            {
                if (Math.Abs(d) < min)
                    min = Math.Abs(d);
            }

            return min;
        }

        /// <summary>
        /// Calculates the inverse of matrix
        /// </summary>
        /// <returns>inverse of this matrix</returns>
        public Matrix Inverse()
        {
            if (!IsSquare())
                throw new InvalidOperationException();

            //seems working good!
            var n = this.rowCount;
            var clone = this.Clone();
            var eye = Eye(n);

            var epsi1on = 1e-10 * MinAbs(clone.coreArray);

            if (epsi1on == 0)
                epsi1on = 1e-9;

            /**/

            var perm = new List<int>();

            var clonea = clone.coreArray;
            var eyea = eye.coreArray;

            for (var j = 0; j < n - 1; j++)
            {
                for (var i = j + 1; i < n; i++)
                {
                    if (System.Math.Abs(clonea[j + j * n]) < epsi1on)
                    {
                        var firstNonZero = -1;

                        for (var k = j + 1; k < n; k++)
                            if (System.Math.Abs(clonea[k + j * n]) > epsi1on)
                                firstNonZero = k;

                        if (firstNonZero == -1)
                            throw new OperationCanceledException();
                        else
                        {
                            clone.SwapRows(firstNonZero, j);
                            eye.SwapRows(firstNonZero, j);

                            perm.Add(j);
                            perm.Add(firstNonZero);
                        }
                    }

                    var alfa = clonea[i + j * n] / clonea[j + j * n];

                    for (var k = 0; k < n; k++)
                    {
                        clonea[i + k * n] -= alfa * clonea[j + k * n];
                        eyea[i + k * n] -= alfa * eyea[j + k * n];
                    }
                }
            }

            /**/

            for (var j = n - 1; j > 0; j--)
            {
                for (var i = j - 1; i >= 0; i--)
                {
                    if (System.Math.Abs(clonea[j + j * n]) < epsi1on)
                    {
                        var firstNonZero = -1;

                        for (var k = j - 1; k >= 0; k--)
                            if (System.Math.Abs(clonea[k + j * n]) > epsi1on)
                                firstNonZero = k;

                        if (firstNonZero == -1)
                            throw new OperationCanceledException();
                        else
                        {
                            clone.SwapRows(firstNonZero, j);
                            eye.SwapRows(firstNonZero, j);

                            perm.Add(j);
                            perm.Add(firstNonZero);
                        }
                    }

                    var alfa = clonea[i + j * n] / clonea[j + j * n];

                    for (var k = n - 1; k >= 0; k--)
                    {
                        clonea[i + k * n] -= alfa * clonea[j + k * n];
                        eyea[i + k * n] -= alfa * eyea[j + k * n];
                    }
                }
            }

            /**/

            for (var i = 0; i < n; i++)
            {
                var alfa = 1 / clonea[i + i * n];

                for (var j = 0; j < n; j++)
                {
                    clonea[i + j * n] *= alfa;
                    eyea[i + j * n] *= alfa;
                }
            }

            /**/

            return eye;
        }
        

        /// <summary>
        /// determines the equation of parameters with fuzzy approach.
        /// </summary>
        /// <remarks>
        /// Because of rounding errors in computer, this method will be used for checking equality of numbers</remarks>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns>true, if d1 equals with fuzzy approach to d2, otherwise false</returns>
        private bool FuzzyEquals(double d1, double d2)
        {
            return Math.Abs(d1 - d2) < Epsilon;
        }

        /// <summary>
        /// The Threshold of equality of two double precision members.
        /// </summary>
        public double Epsilon = 1e-12;
    }
}
#!/usr/bin/env python3

import vtk


file_in = r"F:\UoA\Research\Repo\Rendering\Assets\Resources\datasets\T00.mhd"  # Path to input file
file_out = r"F:\UoA\Research\Repo\Rendering\Assets\Resources\datasets\US01"  # Output file name

# Read input file
reader = vtk.vtkMetaImageReader()
reader.SetFileName(file_in)
reader.Update()

# Perform resize
resize = vtk.vtkImageResize()
resize.SetInputConnection(reader.GetOutputPort())
resize.SetOutputDimensions(64, 64, 64)
resize.Update()

# Downcase to UNSIGNED CHAR
cast = vtk.vtkImageCast()
cast.SetInputConnection(resize.GetOutputPort())
cast.SetOutputScalarTypeToUnsignedChar()
cast.ClampOverflowOn()
cast.Update()

# Write data to MHD file
mhdWriter = vtk.vtkMetaImageWriter()
mhdWriter.SetFileName(file_out + ".mhd")
mhdWriter.SetInputData(cast.GetOutput())
mhdWriter.SetCompression(False)
mhdWriter.Write()

# Print data to output
print(reader.GetOutput())

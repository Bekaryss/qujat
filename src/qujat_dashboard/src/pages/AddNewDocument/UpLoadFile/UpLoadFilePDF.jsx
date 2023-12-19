import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import { FormControl, Input, Button, Box, Typography } from "@mui/material";
import { FaRegWindowClose } from "react-icons/fa";
import instance from "../../../axios/axios";

const UpLoadFilePDF = ({ handleUploadPDF }) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm({
    defaultValues: {
      pdfFile: null,
    },
    mode: "onChange",
  });

  const onSubmit = async (data) => {
    const formData = new FormData();
    formData.append("rq", data[0]);

    // if (data.pdfFile) {
    //   formData.append("pdfFile", data.pdfFile[0]); // Используем правильное имя для PDF файла
    // }

    try {
      const response = await instance.post("/api/1/blobs", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });

      if (response.status === 200) {
        handleUploadPDF(response.data.responseData.id);
      }
    } catch (error) {
      console.error("Произошла ошибка:", error);
    }
  };

  const handleFileChange = (e) => {
    console.log(e.target.files);
    if (e.target.files.length > 0) {
      onSubmit(e.target.files);
    }
  };

  const onChangeClearFile = () => {
    reset({ pdfFile: null });
  };

  useEffect(() => {}, [register, reset]);

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      {/* PDF FILE */}
      <Box sx={{ display: "flex", width: "100%" }}>
        <FormControl sx={{ marginTop: 1, width: "100%" }}>
          <Typography
            className="main_text"
            sx={{ marginBottom: 1, fontStyle: "italic" }}
          >
            PDF
          </Typography>
          <Input
            type="file"
            inputProps={{
              accept: ".pdf",
            }}
            {...register("pdfFile")}
            onChange={handleFileChange}
          />
          {errors.pdfFile && (
            <p style={{ color: "red", marginBottom: "8px" }}>
              {errors.pdfFile.message}
            </p>
          )}
        </FormControl>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            width: "40%",
            marginTop: 1,
            marginLeft: "8px",
          }}
        >
          <FaRegWindowClose
            onClick={() => onChangeClearFile()}
            className="close_icon"
          />
        </Box>
      </Box>
    </form>
  );
};

export default UpLoadFilePDF;

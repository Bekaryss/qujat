import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { FaCheck } from "react-icons/fa";
import {
  FormControl,
  Input,
  Button,
  Box,
  Typography,
  CircularProgress,
} from "@mui/material";
import { FaRegWindowClose } from "react-icons/fa";
import instance from "../../../axios/axios";

const UpLoadFile = ({ onFileUpload }) => {
  const [isFileUpload, setIsUpload] = useState(false);
  const [showProggressUpload, setShowProggresUpload] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm({
    defaultValues: {
      wordFile: null,
    },
    mode: "onChange",
  });

  const onSubmit = async (data) => {
    console.log(data);
    const formData = new FormData();
    formData.append("rq", data[0]);

    try {
      setIsUpload(true);
      const response = await instance.post("/api/1/blobs", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });

      if (response.status === 200) {
        onFileUpload(response.data.responseData.id);
        setInterval(() => {
          setIsUpload(false);
        }, 1500);
      }
    } catch (error) {
      console.error("Произошла ошибка:", error);
      setIsUpload(false);
    }
  };

  const handleFileChange = (e) => {
    if (e.target.files.length > 0) {
      onSubmit(e.target.files);
      setShowProggresUpload(true);
    }
  };

  const onChangeClearFile = () => {
    reset({ wordFile: null });
    setShowProggresUpload(false);
  };

  useEffect(() => {}, [register, reset]);

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      {/* WORD FILE */}
      <Box sx={{ display: "flex", width: "100%" }}>
        <FormControl sx={{ marginTop: 1, width: "100%" }}>
          <Typography
            className="main_text"
            sx={{ marginBottom: 1, fontStyle: "italic" }}
          >
            MS WORD
          </Typography>
          <Input
            type="file"
            inputProps={{
              accept: ".doc, .docx",
            }}
            {...register("wordFile", {
              required: "Выберите файл",
            })}
            onChange={handleFileChange}
          />
          {errors.wordFile && (
            <p style={{ color: "red", marginBottom: "8px" }}>
              {errors.wordFile.message}
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
        {showProggressUpload && (
          <Box sx={{ display: "flex", alignItems: "center" }}>
            {isFileUpload ? (
              <CircularProgress
                size={30}
                sx={{ color: "var(--main-green-500)" }}
              />
            ) : (
              <FaCheck
                style={{ color: "var(--main-green-500)", fontSize: "36px" }}
              />
            )}
          </Box>
        )}
      </Box>
    </form>
  );
};

export default UpLoadFile;

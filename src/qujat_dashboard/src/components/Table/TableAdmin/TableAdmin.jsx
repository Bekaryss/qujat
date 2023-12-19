import React, { useEffect, useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import instance from "../../../axios/axios";
import {
  Table,
  TableContainer,
  TableCell,
  TableRow,
  TableHead,
  TableBody,
  Paper,
  Box,
} from "@mui/material";

import { MdEdit } from "react-icons/md";
import { MdOutlineDeleteSweep } from "react-icons/md";

import DeleteAdmin from "../../Modals/Admin/DeleteAdmin";
import TablePagination from "../../Pagination/TablePagination";

const TableAdmin = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const [adminList, setAdminList] = useState(null);
  const [totalSize, setTotalSize] = useState(0);
  const [pageSize, setPageSize] = useState(0);
  const [pageIndex, setPageIndex] = useState(0);
  const [dataLength, setDataLength] = useState(); //слежение о изменение данных в таблице

  const [isOpenDelete, setIsOpenDelete] = useState(false);
  const [adminId, setAdminId] = useState(0);

  const handleOpenDelete = (id) => {
    setIsOpenDelete(true);
    setAdminId(id);
  };
  const handleCloseDelete = () => {
    setIsOpenDelete(false);
    setDataLength((prev) => prev + 1);
  };

  const handleChangePage = (_, page) => {
    if (page === 0) {
      page += 1;
    }

    navigate(`${location.pathname}?page=${page}`);
  };

  const getAdminList = async () => {
    try {
      const response = await instance.get(
        "/api/1/application-users?pageIndex=0&pageSize=10"
      );

      if (response.status === 200) {
        setTotalSize(response.data.totalSize);
        setPageSize(response.data.pageSize);
        setPageIndex(response.data.pageIndex);
        setAdminList(response.data.responseData);
      }
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    getAdminList();
  }, [dataLength]);

  return (
    <Box className="table" sx={{ marginTop: 2 }}>
      <DeleteAdmin
        deleteItem={isOpenDelete}
        handleCloseDeleteModal={handleCloseDelete}
        id={adminId}
        getAdminList={getAdminList}
      />
      <TableContainer
        component={Paper}
        sx={{ height: `${10 * 72}px`, overflowY: "hidden", boxShadow: "none" }}
      >
        <Table aria-label="simple table">
          <TableHead className="table_content_head">
            <TableRow>
              <TableCell>
                <h2>#</h2>
              </TableCell>
              <TableCell>
                <h2>Электрондық пошта</h2>
              </TableCell>
              <TableCell>
                <h2>Әрекет</h2>
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {adminList?.map((item) => (
              <TableRow
                key={item.id}
                sx={{
                  cursor: "pointer",
                }}
                className="table_item"
              >
                <TableCell>{item.id}</TableCell>
                <TableCell>{item.email}</TableCell>
                <TableCell>
                  <Box className="table_action">
                    <Box
                      sx={{
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        border: "1px solid var(--black)",
                        borderRadius: "8px",
                        padding: 0.5,
                        cursor: "pointer",
                        marginRight: 2,
                        transition: "var(--tran-bg-03)",
                        "&:hover": {
                          background: "var(--main-color-btn)",
                        },
                      }}
                    >
                      <MdEdit style={{ fontSize: "20px" }} />
                    </Box>
                    <Box
                      sx={{
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        border: "1px solid var(--black)",
                        borderRadius: "8px",
                        padding: 0.5,
                        cursor: "pointer",
                        marginRight: 2,
                        transition: "var(--tran-bg-03)",
                        "&:hover": {
                          background: "var(--main-color-btn)",
                        },
                      }}
                      onClick={() => handleOpenDelete(item.id)}
                    >
                      <MdOutlineDeleteSweep
                        style={{
                          fontSize: "20px",
                          color: "var(--main-red)",
                        }}
                      />
                    </Box>
                  </Box>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <Box
        spacing={2}
        sx={{
          width: "100%",
          display: "flex",
          alignContent: "center",
          justifyContent: "center",
          padding: 3,
        }}
      >
        <TablePagination
          totalSize={totalSize}
          pageIndex={pageIndex}
          pageSize={pageSize}
          handleChangePage={handleChangePage}
          renderItem={(item) => (
            <PaginationItem
              component={StyledLink}
              to={`${location.pathname}?page=${item.page}`}
              {...item}
            />
          )}
        />
      </Box>
    </Box>
  );
};

export default TableAdmin;

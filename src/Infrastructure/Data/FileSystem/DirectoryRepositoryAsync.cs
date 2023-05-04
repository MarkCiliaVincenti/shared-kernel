﻿using SharedKernel.Application.UnitOfWorks;
using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Repositories;
using SharedKernel.Infrastructure.Data.FileSystem.UnitOfWorks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Data.FileSystem
{
    /// <summary>
    /// 
    /// </summary>
    public class DirectoryRepositoryAsync : IDirectoryRepositoryAsync
    {
        private readonly FileSystemUnitOfWork _unitOfWork;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        public DirectoryRepositoryAsync(IFileSystemUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork as FileSystemUnitOfWork;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        public Task AddAsync(DirectoryEntity directory, CancellationToken cancellationToken)
        {
            _unitOfWork.Directories.Add(directory);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        public Task<List<string>> GetFilesAsync(string path, CancellationToken cancellationToken)
        {
            var files = Directory.GetFiles(path).ToList();
            return Task.FromResult(files);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        public Task<List<FileEntity>> GetFilesEntitiesAsync(string path, CancellationToken cancellationToken)
        {
            var filesEntities = new ConcurrentBag<FileEntity>();
            Parallel.ForEach(Directory.GetFiles(path), id =>
            {
                var contents = File.ReadAllBytes(path);
                filesEntities.Add(FileEntity.Create(id, Path.GetFileName(id), Path.GetExtension(id),
                    MimeMappingEntity.GetMimeMapping(id), contents));
            });

            return Task.FromResult(filesEntities.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        public Task<List<string>> GetFileNamesAsync(string path, CancellationToken cancellationToken)
        {
            var files = Directory.GetFiles(path).Select(Path.GetFileName).ToList();
            return Task.FromResult(files);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        public Task<bool> ExistsAsync(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Directory.Exists(id));
        }
    }
}
